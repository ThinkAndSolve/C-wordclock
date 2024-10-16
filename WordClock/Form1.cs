using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1 {

	public enum Animations {
		DropDown,
		RandomPixelFill,
		FillPerPixelZigZag,
    FillColumns,
		ClearColumns,
		FillRows,
		ClearRows,
	};

	public class XPanel : Panel {
		public XPanel() {
			this.DoubleBuffered = true;
		}
		private IContainer components;

		private void InitializeComponent() {
			this.SuspendLayout();
			this.ResumeLayout(false);

		}
	}
	public partial class Form1 : Form {
		const int PIXEL_SIZE = 50;
		const int PIXELS_HORIZONTAL = 13;
		const int PIXELS_VERTICAL = 12;
		const int LINE_WIDTH = 2;
		const int BORDER_OFFSET = 3;

		bool showMinuteAnimation = false;
		Animations currentAnimation = Animations.DropDown;
		Brush fontBrush;
    Brush backgroundBrush;

    readonly string[] words = new string[12] {
		//		 0123456789ABC
					"HETDISAEENANO",
					"TWEEDRIEJVIER",
					"VIJFZESZEVENE",
					"ACHTROENEGENQ",
					"TIENELFTWAALF",
					"KWARTZDERTIEN",
					"VEERTIENYVOOR",
					"OVERXHALFBEEN",
					"TWEEUDRIEVIER",
					"VIJFZESPZEVEN",
					"ACHTNEGENTIEN",
					"ELFTWAALFQUUR",
		};

		readonly string[] minuteWords = new string[14] {
			 "EEN*MIN",
			 "TWEE*MIN",
			 "DRIE*MIN",
			 "VIER*MIN",
			 "VIJF*MIN",
			 "ZES*MIN",
			 "ZEVEN*MIN",
			 "ACHT*MIN",
			 "NEGEN*MIN",
			 "TIEN*MIN",
			 "ELF*MIN",
			 "TWAALF*MIN",
			 "DERTIEN*MIN",
			 "VEERTIEN*MIN",
		};

		readonly string[] hourWords = new string[12] {
			 "TWAALF*UUR",
			 "EEN*UUR",
			 "TWEE*UUR",
			 "DRIE*UUR",
			 "VIER*UUR",
			 "VIJF*UUR",
			 "ZES*UUR",
			 "ZEVEN*UUR",
			 "ACHT*UUR",
			 "NEGEN*UUR",
			 "TIEN*UUR",
			 "ELF*UUR",
		};

		Color[,] display = new Color[PIXELS_VERTICAL,PIXELS_HORIZONTAL];
		byte[,] frameBuffer = new byte[PIXELS_VERTICAL, PIXELS_HORIZONTAL];
		ushort[] frameBitBuffer = new ushort[PIXELS_VERTICAL];
		ushort[] newFrameBitBuffer = new ushort[PIXELS_VERTICAL];

		public class WordDescription {
			public int row;
			public int index;
			public int length;
			public WordDescription(int r, int i, int l) {
				row = r;
				index = i;
				length = l;
			}
		}

		private Dictionary<string, WordDescription> dictWordLookup = new Dictionary<string, WordDescription>();


		int height = (PIXELS_VERTICAL * PIXEL_SIZE) + ((PIXELS_VERTICAL + 1) * LINE_WIDTH);
		int width = (PIXELS_HORIZONTAL * PIXEL_SIZE) + ((PIXELS_HORIZONTAL + 1) * LINE_WIDTH);

		private Random random;
		public Form1() {
			InitializeComponent();
			ResizeDrawingPanel();
			InitializeWordDictionary();
			timer1.Enabled = true;
			timer1.Start();
			this.BackColor = Color.Black;
			random = new Random();
			this.Text = "WordClock";
			backgroundBrush = new SolidBrush(Color.FromArgb(5,Color.DarkGray));
			
    }

		private void Form1_Load(object sender, EventArgs e) {
			pnDemo.Paint += PnDemo_Paint;
			fontBrush = GetRandomSolidBrush();
		}

		private void PnDemo_Paint(object sender, PaintEventArgs e) {
			DrawTextInGrid(e);
		}

		byte CheckPixel(byte x, byte y) {
			byte result = 0;
			if ((x < PIXELS_HORIZONTAL) && (y < PIXELS_VERTICAL)) {
				if ((frameBitBuffer[y] & ((ushort)(1 << x))) != 0) {
					result = 1;
				}
      }
			return result;
		}

		void SetPixel(byte x, byte y) {
			if ((x < PIXELS_HORIZONTAL) && (y < PIXELS_VERTICAL)) {
				frameBitBuffer[y] |= (ushort)(1 << x);
			}
		}
		void ClearPixel(byte x, byte y) {
			if ((x < PIXELS_HORIZONTAL) && (y < PIXELS_VERTICAL)) {
				frameBitBuffer[y] &= (ushort)(0xFFFF - (ushort)(1 << x));
			}
		}

		void ClearBitFrame() {
			byte i = 0;
			for (i = 0; i < PIXELS_VERTICAL; i++) {
				frameBitBuffer[i] = 0;
			}
		}

		void SetBitFrame() {
			byte i = 0;
			for (i = 0; i < PIXELS_VERTICAL; i++) {
				frameBitBuffer[i] = 0xFFFF;
			}
		}

		void ClearRow(byte row) {
			if (row < PIXELS_VERTICAL) {
				frameBitBuffer[row] = 0;
			}
		}

		void SetRow(byte row) {
			if (row < PIXELS_VERTICAL) {
				frameBitBuffer[row] = 0xFFFF;
			}
		}

		void ClearColumn(byte column) {
			byte i = 0;
			if (column < PIXELS_HORIZONTAL) {
				for (i = 0; i < PIXELS_VERTICAL; i++) {
					ClearPixel(column, i);
				}
			}
		}
		void SetColumn(byte column) {
			byte i = 0;
			if (column < 16) {
				for (i = 0; i < PIXELS_VERTICAL; i++) {
					SetPixel(column, i);
				}
			}
		}

		private void InitializeWordDictionary() {
			dictWordLookup.Add("HET", new WordDescription(0,0,0));
			dictWordLookup.Add("IS", new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[0], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[1], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[2], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[3], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[4], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[5], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[6], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[7], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[8], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[9], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[10], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[11], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[12], new WordDescription(0, 0, 0));
			dictWordLookup.Add(minuteWords[13], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[0], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[1], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[2], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[3], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[4], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[5], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[6], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[7], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[8], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[9], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[10], new WordDescription(0, 0, 0));
			dictWordLookup.Add(hourWords[11], new WordDescription(0, 0, 0));
			dictWordLookup.Add("KWART", new WordDescription(0, 0, 0));
			dictWordLookup.Add("HALF", new WordDescription(0, 0, 0));
			dictWordLookup.Add("OVER", new WordDescription(0, 0, 0));
			dictWordLookup.Add("VOOR", new WordDescription(0, 0, 0));
			dictWordLookup.Add("UUR", new WordDescription(0, 0, 0));
			LookupWordPositionsAndLength();



		}

		void LookupWordPositionsAndLength() {
			WordDescription tempWordDescription = new WordDescription(0, 0, 0);
			foreach (KeyValuePair<string, WordDescription> kvp in dictWordLookup) {

				if (IsHourDescription(kvp.Key)) {
					//strip and search from end of letter array
					LookupWord(StripMinuteHoursDescrition(kvp.Key), ref tempWordDescription, true);
				} else {
					if (IsMinuteDescription(kvp.Key)) {
						//strip and search from start of letter array
						LookupWord(StripMinuteHoursDescrition(kvp.Key), ref tempWordDescription, false);
					} else {
						//just search form start whithout strip
						LookupWord(kvp.Key, ref tempWordDescription, false);
					}
				}
				kvp.Value.index = tempWordDescription.index;
				kvp.Value.row = tempWordDescription.row;
				kvp.Value.length = tempWordDescription.length;
			}
		}

		static bool IsMinuteDescription(string description) {
			return description.Contains("*MIN");
		}
		static bool IsHourDescription(string description) {
			return description.Contains("*UUR");
		}

		string StripMinuteHoursDescrition(string description) {
			if (description.Contains("*")) {
				return description.Substring(0, description.IndexOf('*'));
			} else {
				return description;
			}
		}

		void LookupWord(string wordToSearch, ref WordDescription descriptionRef, bool searchBackwards) {
			bool wordFound = false;
			int i = (searchBackwards == true) ? words.Length : 0;
			if (searchBackwards == false) {
				while (	(wordFound == false)
						&&	(i < words.Length)
					) {
					if (words[i].Contains(wordToSearch)) {
						//get position and length
						descriptionRef.index = words[i].LastIndexOf(wordToSearch);
						descriptionRef.row = i;
						descriptionRef.length = wordToSearch.Length;
						wordFound = true;
					}
					i++;
				}
			} else {
				while (	(wordFound == false)
						&& (i > 0)
					) {
					if (words[i-1].Contains(wordToSearch)) {
						//get position and length
						descriptionRef.index = words[i-1].LastIndexOf(wordToSearch);
						descriptionRef.row = i-1;
						descriptionRef.length = wordToSearch.Length;
						wordFound = true;
					}
					i--;
				}
			}
		}
		void SetWordByName(string word) {
			int row = dictWordLookup[word].row;
			int length = dictWordLookup[word].length;
			int pos = dictWordLookup[word].index;
			for (int i = pos; i < (pos + length); i++) {
				//frameBuffer[row, i] = 1;
				SetPixel((byte)i, (byte)row);
			}
		}

		void ClearFrame() {
			ClearBitFrame();
		}

					
    void ResizeDrawingPanel() {
			pnDemo.Size = new Size(width+ BORDER_OFFSET, height+ BORDER_OFFSET);
			if (this.Size.Height < pnDemo.Size.Height) {
				this.Size = new Size(this.Size.Width, pnDemo.Size.Height +100);
			}
			if (this.Size.Width < pnDemo.Size.Width) {
				this.Size = new Size(pnDemo.Size.Width + 100, this.Size.Height);
			}
		}

		private Color GetRandomColor() {
			return Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255));
			// The error is here
		}

		public SolidBrush GetRandomSolidBrush() {
			SolidBrush oBrush = new SolidBrush(GetRandomColor());
			return oBrush;
		}

		void DrawTextInGrid(PaintEventArgs e) {
			// Create a StringFormat object with the each line of text, and the block
			// of text centered on the page.
			StringFormat stringFormat = new StringFormat();
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;

			Font font = new Font("Arial", (PIXEL_SIZE ), FontStyle.Bold, GraphicsUnit.Pixel);

			Point fontPoint;
			string character;

			// Draw the text and the surrounding rectangle.
			for (int y = 0; y < PIXELS_VERTICAL + 1; y++) {
				if (y < PIXELS_VERTICAL) {
					for (int x = 0; x < PIXELS_HORIZONTAL + 1; x++) {
						if (x < PIXELS_HORIZONTAL) {
							character = words[y].Substring(x, 1);
							fontPoint = new Point((BORDER_OFFSET + (PIXEL_SIZE / 2) + (x * (PIXEL_SIZE + LINE_WIDTH))), (BORDER_OFFSET + (PIXEL_SIZE / 2) + (y * (PIXEL_SIZE + LINE_WIDTH))));
							if (CheckPixel((byte)x, (byte)y) != 0) {
								e.Graphics.DrawString(character, font, fontBrush, fontPoint, stringFormat);
							} else {
                e.Graphics.DrawString(character, font, backgroundBrush, fontPoint, stringFormat);
              }
						}
					}
				}
			}
			
		}

		void DrawPanelGrid(PaintEventArgs e) {
         Pen gridPen = new Pen(Color.DarkGray, LINE_WIDTH);
			for (int x = 0; x < PIXELS_VERTICAL + 1; x++) {
				e.Graphics.DrawLine(gridPen, (x* (PIXEL_SIZE+LINE_WIDTH))+ BORDER_OFFSET, (BORDER_OFFSET - (LINE_WIDTH / 2)), (x * (PIXEL_SIZE + LINE_WIDTH))+ BORDER_OFFSET, height+ (BORDER_OFFSET - (LINE_WIDTH / 2)));
			}
			for (int y = 0; y < PIXELS_HORIZONTAL + 1; y++) { 
				e.Graphics.DrawLine(gridPen, BORDER_OFFSET - (LINE_WIDTH/2), (y * (PIXEL_SIZE + LINE_WIDTH))+ BORDER_OFFSET, width + (BORDER_OFFSET - (LINE_WIDTH/2)), (y * (PIXEL_SIZE + LINE_WIDTH))+ BORDER_OFFSET);
			}
		}
		bool startNewAnimation = false;
		private void TmAnimation_Tick(object sender, System.EventArgs e) {
			if (showMinuteAnimation) {
				timer1.Enabled = false;
				switch (currentAnimation) {
				case Animations.DropDown:
					showMinuteAnimation = !DropAnimation(startNewAnimation);
					break;
				case Animations.RandomPixelFill:
					showMinuteAnimation = !FillRandomPixelAnimation(startNewAnimation);
               tmAnimation.Interval = 10;
					break;
				case Animations.FillPerPixelZigZag:
					showMinuteAnimation = !FillPerPixelAnimation(startNewAnimation);
					tmAnimation.Interval = 10;
					break;
				case Animations.ClearColumns:
					showMinuteAnimation = !FillClearColumnsAnimation(startNewAnimation,false);
					break;
				case Animations.FillColumns:
					showMinuteAnimation = !FillClearColumnsAnimation(startNewAnimation, true);
					break;
				case Animations.ClearRows:
					showMinuteAnimation = !FillClearRowsAnimation(startNewAnimation, false);
					break;
				case Animations.FillRows:
					showMinuteAnimation = !FillClearRowsAnimation(startNewAnimation, true);
					break;
				default:
					break;
				}
				
				startNewAnimation = false;
				pnDemo.Invalidate();
			} else {
				timer1.Enabled = true;
				tmAnimation.Interval = 50;
				tmAnimation.Enabled = false;
				GenerateTimeFrame();
				if (currentAnimation < Animations.FillRows) {
					currentAnimation++;
				} else {
					currentAnimation = Animations.DropDown;
				}
			}
		}
		#region               

		byte animationStepIndex = 0;
		bool DropAnimation(bool startNew) {
			bool animationReady = false;
			if (startNew) {
				animationStepIndex = 0;
			}
			for (int y = 0; y < PIXELS_VERTICAL; y++) {
					if (y == PIXELS_VERTICAL- 1) {
						frameBitBuffer[0] = 0;
					} else {
						frameBitBuffer[PIXELS_VERTICAL - y - 1] = frameBitBuffer[PIXELS_VERTICAL - y - 2];
					}
			}
			if (animationStepIndex < PIXELS_VERTICAL) {
				animationStepIndex++;
			} else {
				animationReady = true;
			}
			return animationReady;
		}

		bool WipeRightAnimation(bool startNew) {
			bool animationReady = false;
			if (startNew) {
				animationStepIndex = 0;
			}
			
			for (int x = 0; x < PIXELS_HORIZONTAL; x++) {
				for (int y = 0; y < PIXELS_VERTICAL; y++) {
					if (x == PIXELS_HORIZONTAL - 1) {
						frameBuffer[y, 0] = 0;
					} else {
						//TODO
						frameBuffer[y,PIXELS_HORIZONTAL - x - 1] = frameBuffer[y,PIXELS_HORIZONTAL - x - 2];
					}
				}
			}
			if (animationStepIndex < PIXELS_HORIZONTAL) {
				animationStepIndex++;
			} else {
				animationReady = true;
			}
			return animationReady;
		}

		bool frameFilled = false;
		bool FillClearRowsAnimation(bool startNew, bool fill) {
			bool animationReady = false;
			
			if (startNew) {
				frameFilled = false;
				animationStepIndex = 0;
			}
			if (fill) {
				if (!frameFilled) {
					SetRow(animationStepIndex);
				} else {
					ClearRow((byte)(animationStepIndex- PIXELS_VERTICAL));
				}
			} else {
				ClearRow(animationStepIndex);
			}
			
			if (animationStepIndex < (2*PIXELS_VERTICAL)) {
				animationStepIndex++;
				if (animationStepIndex >= PIXELS_VERTICAL) {
					frameFilled = true;
				}
			} else {
				animationReady = true;
			}
			return animationReady;
		}

		bool FillClearColumnsAnimation(bool startNew, bool fill) {
			bool animationReady = false;
			if (startNew) {
				animationStepIndex = 0;
				frameFilled = false;
         }
			if (fill) {
				if (!frameFilled) {
					SetColumn(animationStepIndex);
				} else {
					ClearColumn((byte)(animationStepIndex - PIXELS_HORIZONTAL));
				}
			} else {
				ClearColumn(animationStepIndex);
			}

			if (animationStepIndex < (2 * PIXELS_HORIZONTAL)) {
				animationStepIndex++;
				if (animationStepIndex >= PIXELS_HORIZONTAL) {
					frameFilled = true;
				}
			} else {
				animationReady = true;
			}
			return animationReady;
		}

		Random randX;
		Random randY;
      bool FillRandomPixelAnimation(bool startNew) {
			bool animationReady = false;
			if (startNew) {
				animationStepIndex = 0;
				randX = new Random(42314);
				randY = new Random(3124);

			}



			if (animationStepIndex < 255) {
				animationStepIndex++;
				SetPixel((byte)randX.Next(0, PIXELS_HORIZONTAL-1), (byte)randY.Next(0, PIXELS_VERTICAL-1));
         } else {
				animationReady = true;
			}
			return animationReady;
		}

		byte x;
		byte y;
		bool FillPerPixelAnimation(bool startNew) {
			bool animationReady = false;
			
			if (startNew) {
				x = 0;
				y = 0;
				animationStepIndex = 0;
			}

			if (y < PIXELS_VERTICAL) {
				if (x < PIXELS_HORIZONTAL) {
					if (y % 2 == 0) {
						SetPixel(x, y);
					} else {
						SetPixel((byte)(PIXELS_HORIZONTAL - 1 - x), y);
					}
					x++;
				} else {
					y++;
					x = 0;
				}
			} else {
				animationReady = true;
			}
			 
			return animationReady;
		}
		#endregion

		private void timer1_Tick(object sender, EventArgs e) {
			GenerateTimeFrame();
		}


		bool generateNewColor = false;

		int minDebug = 0;
		void GenerateTimeFrame() {
			bool showMinutes = true;
			random = new Random();

			int minutes = DateTime.Now.ToLocalTime().Minute;
			int realMinutues = minutes;
			int hours = DateTime.Now.ToLocalTime().Hour;

			if (DateTime.Now.ToLocalTime().Second == 0) {
			//if (DateTime.Now.ToLocalTime().Second % 10 == 0) {
				showMinuteAnimation = true;
				startNewAnimation = true;
				tmAnimation.Enabled = true;
				generateNewColor = true;

				return;
			}
			if (!showMinuteAnimation) {
				ClearFrame();
				SetWordByName("HET");
				SetWordByName("IS");

				if (generateNewColor) {
					generateNewColor = false;
					fontBrush = GetRandomSolidBrush();
				}
		
				//determine is "OVER" should be showed
				if (	(realMinutues > 0) && (realMinutues <= 15)
					|| (realMinutues > 30) && (realMinutues < 45)
					) {
					//over
					SetWordByName("OVER");
					minutes = (realMinutues % 15);
				} else {
					//determine is "VOOR" should be showed
					if ((realMinutues > 15) && (realMinutues < 30)
						|| (realMinutues >= 45) && (realMinutues <= 59)
						) {
						//voor
						SetWordByName("VOOR");
						minutes = (15 - (realMinutues % 15));
					} else {
						if (realMinutues == 0) {
							//heel uur
							SetWordByName("UUR");
							showMinutes = false;
						}
					}
				}
				//determine is "HALF" should be showed
				if ((realMinutues > 15) && (realMinutues < 45)) {
					SetWordByName("HALF");
					if (minutes == 30) {
						showMinutes = false;
					}
				}

				if ((realMinutues == 15) || (realMinutues == 45)) {
					SetWordByName("KWART");
					showMinutes = false;
				} 

				if (showMinutes) {
					SetWordByName(minuteWords[minutes - 1]);
				}

				if (realMinutues > 15) {
					hours += 1;
				}
				hours %= 12;
				SetWordByName(hourWords[hours]);
				pnDemo.Invalidate();
			} else {
				//do minute animation
				tmAnimation.Enabled = true;
				tmAnimation.Start();
			}
		}

	}
}
