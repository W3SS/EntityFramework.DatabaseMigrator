﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EntityFramework.DatabaseMigrator
{
    public partial class DatabaseMigrator : BaseDatabaseMigrator
    {
        public DatabaseMigrator()
            : base()
        {
            InitializeComponent();

            LoggerTextBox = txtLog;
        }
    }
}
