using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System;

namespace CellularAutomata
{
    partial class Form1
    {
        private volatile bool continueUpdate = false;
        private volatile int updateRate = 0;
        private Color STATE_DEAD = Color.White;
        private Color STATE_ALIVE = Color.Black;
        private const int ALIVE = 1;
        private const int DEAD = 0;
        public const int MIN_X = 20;
        public const int MIN_Y = 20;
        public const int MAX_X = 450;
        public const int MAX_Y = 450;
        public const int LINE_SPACING = 10;
        public const int NUMBER_OF_LINES = ((MAX_X - MIN_X) / LINE_SPACING) + 1;
        public const int NUMBER_OF_RECTS = NUMBER_OF_LINES - 1;
        private Rect[,] rects = new Rect[NUMBER_OF_RECTS, NUMBER_OF_RECTS];
        private bool rectsInitialized = false;
        static Random random = new Random();
        private Thread updateState;

        private Rect[] GetNeighbors(Rect rect, Rect[] neighborArray)
        {
            /*
             * 
             * NEIGHBOR ARRAY FORMAT
             * X = RECT BEING CONSIDERED/FINDING NEIGHBORS OF
             *      1 2 3      
             *      0 X 4
             *      7 6 5
             * 
             */

            //check if the rect is on a border
            if (rect.x == 0 || rect.y == 0 || rect.y == (NUMBER_OF_RECTS - 1) || rect.x == (NUMBER_OF_RECTS - 1))
            {
                //check if the rect is on a corner
                if (rect.x == 0 && rect.y == 0)
                    neighborArray[1] = rects[NUMBER_OF_RECTS - 1, NUMBER_OF_RECTS - 1];
                else if (rect.x == 0 && rect.y == NUMBER_OF_RECTS - 1)
                    neighborArray[7] = rects[NUMBER_OF_RECTS - 1, 0];
                else if (rect.x == (NUMBER_OF_RECTS - 1) && rect.y == 0)
                    neighborArray[3] = rects[0, NUMBER_OF_RECTS - 1];
                else if (rect.x == (NUMBER_OF_RECTS - 1) && rect.y == (NUMBER_OF_RECTS - 1))
                    neighborArray[5] = rects[0, 0];

                //corner checking is done, now to get the rest of the neighbors
                if (rect.x == 0)
                {
                    neighborArray[0] = rects[NUMBER_OF_RECTS - 1, rect.y];
                    //check if a corner was detected, if not get a neighbor 1
                    if (neighborArray[1] == null)
                        neighborArray[1] = rects[NUMBER_OF_RECTS - 1, rect.y - 1];
                    if (neighborArray[7] == null)
                        neighborArray[7] = rects[NUMBER_OF_RECTS - 1, rect.y + 1];
                }
                if (rect.y == 0)
                {
                    neighborArray[2] = rects[rect.x, NUMBER_OF_RECTS - 1];
                    if (neighborArray[1] == null)
                        neighborArray[1] = rects[rect.x - 1, NUMBER_OF_RECTS - 1];
                    if (neighborArray[3] == null)
                        neighborArray[3] = rects[rect.x + 1, NUMBER_OF_RECTS - 1];
                }
                if (rect.x == (NUMBER_OF_RECTS - 1))
                {
                    neighborArray[4] = rects[0, rect.y];
                    if (neighborArray[3] == null)
                        neighborArray[3] = rects[0, rect.y - 1];
                    if (neighborArray[5] == null)
                        neighborArray[5] = rects[0, rect.y + 1];
                }
                if (rect.y == (NUMBER_OF_RECTS - 1))
                {
                    neighborArray[6] = rects[rect.x, 0];
                    if (neighborArray[5] == null)
                        neighborArray[5] = rects[rect.x + 1, 0];
                    if (neighborArray[7] == null)
                        neighborArray[7] = rects[rect.x - 1, 0];
                }
                //all the wrap-arounds should be done, so the rest of the neighbors can be gotten normally below
            }
            //if it is not on the border then just get the neighbors normally
            //if the rect was on a border all of the wraparounds should be done, so get neighbors if they are null
            if (neighborArray[0] == null) neighborArray[0] = rects[rect.x - 1, rect.y];
            if (neighborArray[1] == null) neighborArray[1] = rects[rect.x - 1, rect.y - 1];
            if (neighborArray[2] == null) neighborArray[2] = rects[rect.x, rect.y - 1];
            if (neighborArray[3] == null) neighborArray[3] = rects[rect.x + 1, rect.y - 1];
            if (neighborArray[4] == null) neighborArray[4] = rects[rect.x + 1, rect.y];
            if (neighborArray[5] == null) neighborArray[5] = rects[rect.x + 1, rect.y + 1];
            if (neighborArray[6] == null) neighborArray[6] = rects[rect.x, rect.y + 1];
            if (neighborArray[7] == null) neighborArray[7] = rects[rect.x - 1, rect.y + 1];
            
            return neighborArray;
        }


        private void NextState()
        {
            int numberOfAliveNeighbors;
            foreach (Rect rect in rects)
            {
                numberOfAliveNeighbors = 0;
                Rect[] neighbors = new Rect[8]; //8 neighbors in conway's game of life
                neighbors = GetNeighbors(rect,neighbors);
                foreach (Rect r in neighbors)
                {
                    if (r.state == 1) numberOfAliveNeighbors += 1;
                }

                //rules of conway's game of life: (source: wikipedia: http://en.wikipedia.org/wiki/Conway%27s_Game_of_Life)
                //  -Any live cell with fewer than two live neighbours dies, as if caused by under-population.
                //  -Any live cell with two or three live neighbours lives on to the next generation.
                //  -Any live cell with more than three live neighbours dies, as if by overcrowding.
                //  -Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

                if (rect.state == 1) //check alive rules
                {
                    switch (numberOfAliveNeighbors)
                    {
                        case 2: //fall through to case 3 because they achieve the same result
                        case 3:
                            rect.nextState = ALIVE;
                            break;
                        default: //any amount of neighbors that isn't 2 or 3 causes death
                            rect.nextState = DEAD;
                            break;
                    }
                }
                else
                {   //dead rule
                    if (numberOfAliveNeighbors == 3)
                        rect.nextState = ALIVE;
                    else
                        rect.nextState = DEAD;
                }

                
            }
            //finally, update the state of each rect to its next state
            foreach (Rect r in rects)
            {
                r.state = r.nextState;
                r.nextState = 0;
            }
        }

        private void InitializeRects()
        {
            if (rectsInitialized)
            {
                return;
            }
            rectsInitialized = true;
            for (int iteratorX = 0; iteratorX < NUMBER_OF_RECTS; iteratorX++)
            {
                for (int iteratorY = 0; iteratorY < NUMBER_OF_RECTS; iteratorY++)
                {
                    Rect temp = new Rect();
                    temp.x = iteratorX;
                    temp.y = iteratorY;
                    //rectangles need to be offset by a pixel, and are one less than Form1.LINE_SPACING in dimensions
                    //this is so the rectangles don't interfere/overwrite the lines
                    temp.minX = MIN_X + ((iteratorX * Form1.LINE_SPACING) + 1);
                    temp.minY = MIN_Y + ((iteratorY * Form1.LINE_SPACING) + 1);
                    temp.maxX = MIN_X + ((iteratorX * Form1.LINE_SPACING) + Rect.DIMENSION_X);
                    temp.maxY = MIN_Y + ((iteratorY * Form1.LINE_SPACING) + Rect.DIMENSION_Y);
                    rects[iteratorX, iteratorY] = temp;
                }
            }
        }

        private void SetupInitialState()
        {
            int probability = (int)numericUpDown1.Value; //probability that a cell/rectangle is "alive" or in state 1
            foreach (Rect rect in rects)
            {
                if (random.Next(1, 100) <= probability) //
                {
                    rect.state = 1;
                }
                else
                {   //clear values that could be left behind from previous iterations when setting up the initial state
                    rect.state = 0;
                }
            }
        }

        private void DrawRects()
        {
            //SolidBrush aliveState = new SolidBrush(STATE_ALIVE);
           // SolidBrush deadState = new SolidBrush(STATE_DEAD);
            SolidBrush[] stateBrush = new SolidBrush[2];
            stateBrush[0] = new SolidBrush(STATE_DEAD);
            stateBrush[1] = new SolidBrush(STATE_ALIVE);
            Graphics rectGraphic = this.CreateGraphics();
            foreach(Rect rect in rects)
            {
                 rectGraphic.FillRectangle(stateBrush[rect.state], rect.minX, rect.minY, LINE_SPACING - 1, LINE_SPACING - 1);   
            }
            foreach (SolidBrush brush in stateBrush)
            {   //make the dispose work for any number of brushes in case more colors are added.
                brush.Dispose();
            }
           // deadState.Dispose();
            //aliveState.Dispose();
            rectGraphic.Dispose();
        }

        private void DrawLines()
        {
            //draw lines on the form
            Pen drawLines = new Pen(Color.Black);
            Graphics formLines = this.CreateGraphics();
            //formLines.DrawLine(drawLines, 10, 10, 100, 100);
            for (int i = 0; i < NUMBER_OF_LINES; i++)
            {
                formLines.DrawLine(drawLines, MIN_X + (i * LINE_SPACING),
                                             MIN_Y,
                                             MIN_X + (i * LINE_SPACING),
                                             MAX_Y);
                formLines.DrawLine(drawLines, MIN_X,
                                             MIN_Y + (i * LINE_SPACING),
                                             MAX_X,
                                             MIN_Y + (i * LINE_SPACING));
            }
            drawLines.Dispose();
            formLines.Dispose();
        }

    
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(461, 64);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(461, 202);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "Seed World";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(461, 122);
            this.trackBar1.Maximum = 15;
            this.trackBar1.Minimum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(99, 45);
            this.trackBar1.TabIndex = 3;
            this.trackBar1.Value = 1;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(461, 289);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown1.TabIndex = 4;
            this.numericUpDown1.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(458, 106);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "1 Update per second";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(458, 247);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(103, 39);
            this.label2.TabIndex = 6;
            this.label2.Text = "Percent chance that a cell initializes as alive (black).";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 471);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private Button button2;
        private TrackBar trackBar1;
        private NumericUpDown numericUpDown1;
        private Label label1;
        private Label label2;

    }
}

