using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string sBusLimit = textBox1.Text;
            string sBranchNum =  textBox2.Text;

            string[] sBranchStakeNum = new string[5];
            sBranchStakeNum[0] = textBox3.Text;
            sBranchStakeNum[1] = textBox4.Text;
            sBranchStakeNum[2] = textBox5.Text;
            sBranchStakeNum[3] = textBox6.Text;
            sBranchStakeNum[4] = textBox7.Text;
            string[] sBranchLimit = new string[5];

            sBranchLimit[0] = textBox13.Text;
            sBranchLimit[1] = textBox14.Text;
            sBranchLimit[2] = textBox15.Text;
            sBranchLimit[3] = textBox16.Text;
            sBranchLimit[4] = textBox17.Text;

            string[] sBranchReserve = new string[5];
            sBranchReserve[0] = textBox21.Text;
            sBranchReserve[1] = textBox22.Text;
            sBranchReserve[2] = textBox23.Text;
            sBranchReserve[3] = textBox24.Text;
            sBranchReserve[4] = textBox25.Text;

            int[] iBranchReserve = new int[5];


            int iBusLimit;
            int iBusDistriLimit=0;
            int iBranchNum;
            int[] iBranchStakeNum = new int[5];
            int[] iBranchOutCurr = new int[5];
            int[] iBranchOutCurrPlus = new int[5];
            int iBranchCurrTotal = 0;
            int[] iBranchLimit = new int[5];
            int[] iBranchDistribLimit = new int[5];
            bool[] iBranchLimitArrive = new bool[5];
            
            bool isValid = false;
            int StakeNumTotal=0;
            int iBranchLimitSum = 0;
            isValid = int.TryParse(sBusLimit,out iBusLimit);
            if (!isValid ) 
            {                
                return;
            }
            iBusDistriLimit = iBusLimit;
            isValid = int.TryParse(sBranchNum, out iBranchNum);
            if (!isValid)
            {
                return;
            }
           // iBusLimit -= iBranchNum;
            if (iBranchNum>0 && iBranchNum<=5)
            {
                for(int i = 0;i< iBranchNum;i++)
                {
                    isValid = int.TryParse(sBranchStakeNum[i], out iBranchStakeNum[i]);
                    if (!isValid)
                    {
                        return;
                    }
                    isValid = int.TryParse(sBranchLimit[i], out iBranchLimit[i]);
                    if (!isValid)
                    {
                        return;
                    }
                    isValid = int.TryParse(sBranchReserve[i], out iBranchReserve[i]);
                    if (!isValid)
                    {
                        return;
                    }

                    StakeNumTotal += iBranchStakeNum[i];

                    iBranchDistribLimit[i] = (iBranchLimit[i] - iBranchReserve[i]);                   
                    iBusDistriLimit -= iBranchReserve[i];
                    iBranchLimitSum += iBranchLimit[i];

                }
            }
            else
            {
                return;
            }

            textBox26.Text = iBusDistriLimit.ToString();

            int SurplusLimit = 0;
            int SurplusStakeNum = 0;
            SurplusLimit = iBusDistriLimit;
            SurplusStakeNum = StakeNumTotal;
            int iBranchOutCurrTemp = 0;
            int OneCycleCurrent=0;
            int OneCycleNum=0;
            ushort[] wCRCTalbeAbs= new ushort[16];
            ushort polynomial = 0x1021; // 多项式 x^16 + x^12 + x^5 + 1
            ushort k;
            for (k = 0; k < 16; k++)
            {
                ushort crc = k; // 初始化为低4位的值
                for (int j = 0; j < 4; j++)
                { // 每次处理1位，共处理4位
                    if ((ushort)(crc & 1)>0)
                    {
                        crc = (ushort)((crc >> 1) ^ polynomial); // 如果最低位为1，异或多项式
                    }
                    else
                    {
                        crc = (ushort)(crc >> 1); // 否则只右移
                    }
                }
                wCRCTalbeAbs[k] = crc; // 将结果存入表中
            }





            while ((SurplusLimit > 0) && SurplusStakeNum > 0) 
            { 
                for (int i = 0; i < iBranchNum; i++)
                {
                    if(iBranchLimitArrive[i] == true)
                    {
                        continue;
                    }
                    iBranchOutCurrTemp = SurplusLimit * iBranchStakeNum[i] / SurplusStakeNum;
                    if(iBranchOutCurrTemp == 0 && iBranchStakeNum[i]!=0)
                    {
                        iBranchOutCurrTemp = 1;
                    }
                    iBranchOutCurr[i] += iBranchOutCurrTemp;
                    if (iBranchOutCurr[i] >= iBranchDistribLimit[i])
                    {
                        iBranchLimitArrive[i] = true;
                        OneCycleNum += iBranchStakeNum[i];
                        iBranchOutCurrTemp -= (iBranchOutCurr[i] - iBranchDistribLimit[i]);
                        iBranchOutCurr[i] = iBranchDistribLimit[i];
                    }
                    else if(iBranchOutCurr[i]<6)
                    {
                       // iBranchLimitArrive[i] = true;
                      //  iBranchOutCurr[i] = 0;
                       // iBranchOutCurrTemp = 0;
                       // OneCycleNum += iBranchStakeNum[i];
                    }
                    OneCycleCurrent += iBranchOutCurrTemp;
                    if(SurplusLimit <= OneCycleCurrent || SurplusStakeNum<= OneCycleNum)
                    {
                        break;
                    }
                }
                SurplusLimit -= OneCycleCurrent;
                SurplusStakeNum -= OneCycleNum;
                OneCycleCurrent = 0;
                OneCycleNum = 0;
            }
            for (int i = 0; i < iBranchNum; i++)
            {
                iBranchOutCurrPlus[i] = (iBranchOutCurr[i] + iBranchReserve[i]);
            }
            textBox8.Text = iBranchOutCurrPlus[0].ToString();
            textBox9.Text = iBranchOutCurrPlus[1].ToString();
            textBox10.Text = iBranchOutCurrPlus[2].ToString();
            textBox11.Text = iBranchOutCurrPlus[3].ToString();
            textBox12.Text = iBranchOutCurrPlus[4].ToString();

            for (int i = 0; i < iBranchNum; i++)
            {
                iBranchCurrTotal += iBranchOutCurrPlus[i];
            }
            textBox18.Text = iBranchCurrTotal.ToString(); 
            textBox20.Text = StakeNumTotal.ToString();
            textBox19.Text = iBranchLimitSum.ToString();
            //Console.WriteLine(isValid);


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void label20_Click(object sender, EventArgs e)
        {

        }

        private void label18_Click(object sender, EventArgs e)
        {

        }

        private void label19_Click(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
    }
}
