﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HartSDK;

namespace HartTool
{
    public partial class Frm性能参数 : Form, IHartCommunication
    {
        public Frm性能参数()
        {
            InitializeComponent();
        }

        #region 实现接口 IHartCommunication
        public HartSDK.HartDevice HartDevice { get; set; }

        public void ReadData()
        {
            btnWrite.Enabled = HartDevice != null && HartDevice.IsConnected;
            button1.Enabled = HartDevice != null && HartDevice.IsConnected;
            OutputInfo oi = HartDevice.ReadOutput();
            if (oi != null)
            {
                cmbTranserFunction.SelectedIndex = oi.TransferFunctionCode;
                txtDampValue.Text = oi.DampingValue.ToString();
                cmbPVUnit.SelectedIndex = (int)oi.PVUnitCode;
            }
            txtTrim4.Text = HartDevice.ReadCurrentTrim(0).ToString();
        }
        #endregion

        private bool CheckInput()
        {
            if (cmbTranserFunction.SelectedIndex < 0)
            {
                MessageBox.Show("没有指定输出函数");
                return false;
            }
            if (cmbPVUnit.SelectedIndex <= 0)
            {
                MessageBox.Show("没有指定显示单位");
                return false;
            }
            decimal damp = 0;
            if (!decimal.TryParse(txtDampValue.Text, out damp))
            {
                MessageBox.Show("输入的阻尼系数不是有效的数值", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        private void btnWrite_Click(object sender, EventArgs e)
        {
            if (!CheckInput()) return;
            bool ret = false;

            TransferFunctionCode tc = (TransferFunctionCode)cmbTranserFunction.SelectedIndex;
            ret = HartDevice.WriteTransferFunction( tc);
            if (!ret) MessageBox.Show(HartDevice.GetLastError(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            UnitCode pvUnit = (UnitCode)cmbPVUnit.SelectedIndex;
            ret = HartDevice.WritePVUnit( pvUnit);
            if (!ret) MessageBox.Show(HartDevice.GetLastError(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

            decimal damp = 0;
            decimal.TryParse(txtDampValue.Text, out damp);
            ret = HartDevice.WriteDampValue( (float)damp);
            if (!ret)
            {
                MessageBox.Show(HartDevice.GetLastError(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float f = 0;
            if (float.TryParse(txtTrim4.Text, out f))
            {
                bool ret = HartDevice.WriteCurrentTrim( 0x22, f);
                if (!ret)
                {
                    MessageBox.Show(HartDevice.GetLastError(), "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("小信号切除量输入的值不正确", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
