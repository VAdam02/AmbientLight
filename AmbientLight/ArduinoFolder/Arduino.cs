using AmbientLight.Power;
using AmbientLight.Strip.LEDs;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace AmbientLight
{
	class Arduino
	{
		public const byte FlushID = 255;	//255 ; pin ; 0 ; 0; 0
		public const byte FlushID_Alt = 7;
		public const byte CheckConnectionID = 254;  //254 ; test ; 0 ; 0; 0
		public const byte BrigthnessID = 253;	//253 ; pin ; brightness ; 0; 0
		public const byte BrigthnessID_Alt = 6;

		public static Arduino instance;

		Logger logger;
		SerialPort serial;
		
		public static Arduino Instantiate()
		{
			instance = new Arduino();
			return instance;
		}

		Arduino()
		{
			logger = new Logger();
			logger.AddLevel(nameof(Arduino));

			queue = new List<LED>();

			logger.Log("Looking for an Arduino...");

			try { serial = ArduinoFinder.FindArduino(); }
			catch (ArduinoNotFoundException exception)
			{
				logger.Error("No compatible AmbientLight controller founded");
				throw exception;
			}
			logger.Log("Compatible AmbientLight controller founded at " + serial.PortName);

			flusher = new Thread(() => Flusher(this));
			StartFlush();
		}

		#region Flusher
		private List<LED> queue;
		public int GetQueueSize()
		{
			return queue.Count;
		}

		public void SetBrightness(byte brightness, byte pin, Voltage normalizedVoltage) //normalized voltage: I mean that voltage is need to be on 100% and the others must be at 0-100%
		{
			LED led = LED.GetLED(BrigthnessID_Alt, pin);
			led.SetRGB(brightness, 0, 0);
			SendRGB(led);

			BrightnessChangeEvent changeEvent = new BrightnessChangeEvent(pin, brightness, normalizedVoltage);
			changeEvent.Activate();
		}
		public void Flush(byte pin)
		{
			SendRGB(LED.GetLED(FlushID_Alt, pin));
		}
		public void SendRGB(LED led)
		{
			queue.Add(led);
		}

		private Thread flusher;
		private bool running;
		public static void Flusher(Arduino arduino)
		{
			int error = 0;
			bool changed;
			Logger logger = new Logger(arduino.logger);
			logger.AddLevel("Flusher");

			logger.Debug(DebugCategory.Rare, "Data flushing started");

			while (arduino.running)
			{
				while (arduino.running && arduino.queue.Count < 1) { Thread.Sleep(1); }

				try
				{
					if (arduino.queue.Count > 0)
					{
						changed = arduino.queue[0].GetRGB(out byte[] data);
						if (changed)
						{
							if (data[0] == FlushID_Alt)
							{
								//TODO debug...
								logger.Debug(DebugCategory.Spammer, "Flush:\t" + data[0] + "\t" + data[1] + "\t" + data[2] + "\t" + data[3] + "\t" + data[4]);
								data[0] = FlushID;
							}
							else if (data[0] == BrigthnessID_Alt)
							{
								//TODO debug...
								logger.Debug(DebugCategory.Spammer, "Brightness:\t" + data[0] + "\t" + data[1] + "\t" + data[2] + "\t" + data[3] + "\t" + data[4]);
								data[0] = BrigthnessID;
							}
							else
							{
								//TODO debug...
								//logger.Debug(DebugCategory.Spammer, "Original:\t" + data[0] + "\t" + data[1] + "\t" + data[2] + "\t" + data[3] + "\t" + data[4]);

								LED.leds[BrigthnessID_Alt][data[0]].GetRGB(out byte[] cache);

								double alpha = PowerManager.instance.GetAlpha(arduino.queue[0].GetVoltage());

								if (data[0] == 3 && data[1] == 11)
								{
									//TODO debug...
									//logger.Error((alpha * 255) + "\t" + cache[2] + "\t" + data[2] + "\t" + data[3] + "\t" + data[4]);
								}

								if (cache[2] < 1) { cache[2] = 1; }


								double multiplier = alpha * 255 / cache[2];
								if (multiplier > 1) { multiplier = 1; }


								data[2] = (byte)(multiplier * data[2]);
								data[3] = (byte)(multiplier * data[3]);
								data[4] = (byte)(multiplier * data[4]);


								//TODO debug...
								//logger.Debug(DebugCategory.Spammer, "Transformed:\t" + data[0] + "\t" + data[1] + "\t" + data[2] + "\t" + data[3] + "\t" + data[4]);
							}

							arduino.serial.Write(new byte[] { data[0], data[1], data[2], data[3], data[4] }, 0, 5);
						}
						arduino.queue[0].Written();
					}
					arduino.queue.RemoveAt(0);
					error = 0;
				}
				catch
				{
					error++;
					if (error >= 3)
					{
						arduino.queue.RemoveAt(0);
						error = 0;
					}
				}
			}

			logger.Debug(DebugCategory.Rare, "Data flushing stopped");
		}

		private void StartFlush()
		{
			running = true;
			flusher.Start();
		}

		private void StopFlush()
		{
			running = false;
		}
		#endregion
	}

	#region ArduinoFinder
	class ArduinoFinder
	{
		public static SerialPort FindArduino()
		{
			return FindArduino(3);
		}
		public static SerialPort FindArduino(int trycount) 
		{
			bool success = false;
			int tries = 0;
			while (!success && tries < trycount)
			{
				int portID = 0;
				while (!success && portID < 255)
				{
					try
					{
						SerialPort testserial;
						testserial = new SerialPort("COM" + portID) { BaudRate = 19200 };
						testserial.Open();

						success = CheckVersion(testserial);

						if (success) { return testserial; }
						else { throw new ArduinoNotFoundException(); }
					}
					catch (Exception) { }
					portID++;
				}
				tries++;
			}

			throw new ArduinoNotFoundException();
		}

		public static bool CheckVersion(SerialPort serial) { return CheckVersion(serial, 1000); }
		public static bool CheckVersion(SerialPort serial, int timeout)
		{
			long endtime = DateTimeOffset.Now.ToUnixTimeMilliseconds() + timeout;
			byte[] check = new byte[] { 0, 0, 0, 0, 0 };
			Random rnd = new Random();
			check[0] = Arduino.CheckConnectionID;
			check[1] = (byte)rnd.Next(0, 63);
			serial.Write(check, 0, 5);

			while (serial.BytesToRead < 5 && DateTimeOffset.Now.ToUnixTimeMilliseconds() < endtime) { }
			if (serial.BytesToRead < 5)
			{
				serial.ReadExisting();
				return false;
			}

			serial.Read(check, 0, 5);
			int[] get = new int[] { check[1], check[2] };

			//2.2
			//TODO if (get[0] * 2 + 2 == get[1]) { return true; }
			if (get[0] * 2 + 0 == get[1]) { return true; }

			serial.ReadExisting();
			return false;
		}
	}
	#endregion
}
