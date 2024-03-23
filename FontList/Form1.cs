using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using static System.Net.Mime.MediaTypeNames;

namespace FontList
{
    public partial class Form1 : Form
    {
        private FontFamily[] fontFamilies = null;
        private int fontSize = 20;
        private int textHeight = 40;
        private int textWidth = 0;

        private int startDrawRow = 0;
        
        //总共需要绘制几列
        private int numColumns = 1;

        public Form1()
        {
            InstalledFontCollection fontCollection = new InstalledFontCollection();
            fontFamilies = fontCollection.Families;

            // 获取字体文本最大长度
            this.textWidth = this.GetMaxFontNameWidth();

            InitializeComponent();
            this.MouseWheel += Form1_MouseWheel;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (e.Delta > 0) this.fontSize = Math.Min(100, this.fontSize + 10);
                else this.fontSize = Math.Max(12, this.fontSize - 10);
            } else
            {
                if (e.Delta > 0) this.vScrollBar1.Value = Math.Max(this.vScrollBar1.Value - 5, this.vScrollBar1.Minimum);
                else this.vScrollBar1.Value = Math.Min(this.vScrollBar1.Value + 5, this.vScrollBar1.Maximum - this.vScrollBar1.LargeChange + 1);
            }
            Recalculate();
            Invalidate();

        }

        private void Recalculate()
        {
            this.textWidth = this.GetMaxFontNameWidth();
            this.textHeight = this.fontSize * 2;

            // 计算需要几列
            this.numColumns = Math.Max((int)Math.Floor((double)this.ClientSize.Width / this.textWidth), 1);

            // 计算滚动条的属性
            // Maximum 表示索引
            this.vScrollBar1.Maximum = Math.Max(
                0,
                (int)Math.Ceiling((double)fontFamilies.Length / this.numColumns) - 1
            );
            this.vScrollBar1.LargeChange = Math.Min(
                (int)Math.Floor((double)this.ClientSize.Height / this.textHeight),
                this.vScrollBar1.Maximum + 1
            );
            this.vScrollBar1.SmallChange = 1;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            this.DoubleBuffered = true;

            float x = 0.0f;
            float y = 0.0f;
            for(int c = 0; c < this.numColumns; c++)
            {
                y = 0.0f;
                int indexInAllFontFamilies = c * (this.vScrollBar1.Maximum + 1) + this.startDrawRow;
                for (int r = 0; r < this.vScrollBar1.LargeChange; r++)
                {
                    int idx = r + indexInAllFontFamilies;
                    if (idx >= fontFamilies.Length) break;
                    FontFamily fm = fontFamilies[idx];
                    Font font = new Font(fm, this.fontSize);
                    e.Graphics.DrawString($"{(idx+1).ToString()}: {font.Name}", font, Brushes.Black, x, y);
                    font.Dispose();
                    y += this.textHeight;
                }
                x += this.textWidth * 1.2f;
            }
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            this.startDrawRow = this.vScrollBar1.Value;
            Invalidate();
            Debug.Print(this.vScrollBar1.Value.ToString());
        }

        private int GetMaxFontNameWidth()
        {
            int width = 0;
            foreach(FontFamily fm in fontFamilies) { 
                Font font = new Font(fm, this.fontSize);
                width = Math.Max(TextRenderer.MeasureText(font.Name, font).Width, width);
                font.Dispose();
            }
            return width;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            Recalculate();
            Invalidate();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PageUp) { this.vScrollBar1.Value = Math.Max(this.vScrollBar1.Minimum, this.vScrollBar1.Value - this.vScrollBar1.LargeChange); Recalculate();Invalidate(); }
            else if (e.KeyCode == Keys.PageDown) { this.vScrollBar1.Value = Math.Min(this.vScrollBar1.Maximum + 1 - this.vScrollBar1.LargeChange, this.vScrollBar1.Value + this.vScrollBar1.LargeChange); Recalculate(); Invalidate(); }
        }
    }
}
