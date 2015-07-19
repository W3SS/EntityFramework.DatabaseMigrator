﻿using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Infrastructure;
using System.Linq;
using System.Windows.Forms;
using EntityFramework.DatabaseMigrator.Migrations;

namespace EntityFramework.DatabaseMigrator
{
    public partial class DatabaseMigrator : BaseDatabaseMigrator
    {
        private MigratorLoggingDecorator _currentMigrator;
        private string _currentPending;
        private string _currentCompleted;

        public DatabaseMigrator()
            : base()
        {
            InitializeComponent();

            LoggerTextBox = txtLog;

            MigrationTargetChanged += DatabaseMigrator_MigrationTargetChanged;
            MigrationCompleted += DatabaseMigrator_MigrationCompleted;
        }

        protected override void LoadMigrators()
        {
            base.LoadMigrators();

            if (Migrators.Count() > 1)
            {
                lblMigrationTarget.Visible = false;

                cmbMigrationTarget.Visible = true;
                cmbMigrationTarget.DataSource = new BindingSource(Migrators, null);
                cmbMigrationTarget.DisplayMember = "Key";
                cmbMigrationTarget.ValueMember = "Key";
            }
            else if (Migrators.Count() == 1)
            {
                cmbMigrationTarget.Visible = false;
                lblMigrationTarget.Visible = true;
            }
        }

        private void UpdateMigrationLists(DbMigratorEventArgs e)
        {
            lbPending.DataSource = e.PendingMigrations;
            btnMigrateSql.Enabled = e.PendingMigrations.Any();
            btnMigrate.Enabled = btnMigrateSql.Enabled;

            lbCompleted.DataSource = e.CompletedMigrations;
            btnRollbackSql.Enabled = e.CompletedMigrations.Any();
            btnRollback.Enabled = btnRollbackSql.Enabled;
            btnMigrationHistory.Enabled = btnRollbackSql.Enabled;
        }

        private void DatabaseMigrator_MigrationTargetChanged(object sender, MigrationTargetChangedEventArgs e)
        {
            _currentMigrator = e.Migrator;

            lblMigrationTarget.Text = e.MigratorTitle;
            cmbMigrationTarget.SelectedText = e.MigratorTitle;

            UpdateMigrationLists(e);
        }

        private void DatabaseMigrator_MigrationCompleted(object sender, DbMigratorEventArgs e)
        {
            UpdateMigrationLists(e);
        }

        private void lbPending_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentPending = (string)lbPending.SelectedItem;
            btnMigrateSql.Text = "View Sql For Migration To " + _currentPending;
            btnMigrate.Text = "Migrate to " + _currentPending;

            OnPendingMigrationTargetChanged(new MigrationChangedEventArgs(_currentMigrator, _currentPending));
        }

        private void lbCompleted_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentCompleted = (string)lbCompleted.SelectedItem;
            btnRollbackSql.Text = "View Sql For Rollback To " + _currentCompleted;
            btnRollback.Text = "Rollback to " + _currentCompleted;
            btnMigrationHistory.Text = "View Migration History For " + _currentCompleted;

            OnCompletedMigrationChanged(new MigrationChangedEventArgs(_currentMigrator, _currentPending));
        }

        private void btnMigrateSql_Click(object sender, EventArgs e)
        {
            Logger.WriteLine();
            Logger.WriteLine(GetMigrationSql(_currentMigrator, _currentPending));
        }

        private void btnRollbackSql_Click(object sender, EventArgs e)
        {
            Logger.WriteLine();
            Logger.WriteLine(GetMigrationSql(_currentMigrator, _currentCompleted));
        }

        private void btnRollback_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to rollback to " + _currentCompleted + "?", "Rollback Migration", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                ExecuteMigration(_currentMigrator, _currentCompleted);
            }
        }

        private void btnMigrate_Click(object sender, EventArgs e)
        {
            ExecuteMigration(_currentMigrator, _currentPending);
        }

        private void btnMigrationHistory_Click(object sender, EventArgs e)
        {
            Logger.WriteLine();
            Logger.WriteLine(GetMigrationHistory(_currentMigrator, _currentCompleted));
        }

        private void btnRollbackAllSql_Click(object sender, EventArgs e)
        {
            Logger.WriteLine();
            Logger.WriteLine(GetRollbackAllSql(_currentMigrator));
        }

        private void btnRollbackAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to rollback all migrations?", "Rollback All Migrations", MessageBoxButtons.YesNo, MessageBoxIcon.Stop) == DialogResult.Yes)
            {
                RollbackAll(_currentMigrator);
            }
        }

        private void btnClearLog_Click(object sender, EventArgs e)
        {
            txtLog.Text = "";
        }

        private void btnReseed_Click(object sender, EventArgs e)
        {
            Logger.WriteLine();
            Logger.WriteLine("Reseeding...");
            Reseed(_currentMigrator);
            Logger.WriteLine("Reseed complete.");
        }
    }
}
