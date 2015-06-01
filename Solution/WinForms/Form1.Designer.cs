namespace WinForms
{
	partial class Form1
	{
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
			if(disposing && (components != null))
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.Label label5;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label6;
			System.Windows.Forms.Label label7;
			System.Windows.Forms.Label label8;
			System.Windows.Forms.Label label9;
			System.Windows.Forms.Label label14;
			System.Windows.Forms.Label label15;
			System.Windows.Forms.Label label11;
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.mainTabControl = new System.Windows.Forms.TabControl();
			this.tabDailyExpenses = new System.Windows.Forms.TabPage();
			this.panel2 = new System.Windows.Forms.Panel();
			this.dailyExpensesDgv = new System.Windows.Forms.DataGridView();
			this.TitleDailyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AmountDailyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QuantityDailyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.CategoryDailyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.CommentDailyColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DailyExpensesBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
			this.cbExpenseUnit = new System.Windows.Forms.ComboBox();
			this.UnitBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.tbExpenseComment = new System.Windows.Forms.TextBox();
			this.tbExpenseQuantity = new System.Windows.Forms.TextBox();
			this.cbExpenseCategory = new System.Windows.Forms.ComboBox();
			this.CategoryBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.tbExpenseAmount = new System.Windows.Forms.TextBox();
			this.tbExpenseTitle = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
			this.button3 = new System.Windows.Forms.Button();
			this.button5 = new System.Windows.Forms.Button();
			this.button6 = new System.Windows.Forms.Button();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.label1 = new System.Windows.Forms.Label();
			this.labelDailySummary = new System.Windows.Forms.Label();
			this.tabMonthlyExpenses = new System.Windows.Forms.TabPage();
			this.monthlyExpensesDgv = new System.Windows.Forms.DataGridView();
			this.Date = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MonthlyExpensesBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
			this.label2 = new System.Windows.Forms.Label();
			this.labelMonthlySummary = new System.Windows.Forms.Label();
			this.tabMonthlyIncomes = new System.Windows.Forms.TabPage();
			this.monthlyIncomesDgv = new System.Windows.Forms.DataGridView();
			this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dataGridViewTextBoxColumn10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MonthlyIncomesBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
			this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
			this.tbIncomeTitle = new System.Windows.Forms.TextBox();
			this.tbIncomeAmount = new System.Windows.Forms.TextBox();
			this.tbIncomeComment = new System.Windows.Forms.TextBox();
			this.flowLayoutPanel7 = new System.Windows.Forms.FlowLayoutPanel();
			this.button7 = new System.Windows.Forms.Button();
			this.button9 = new System.Windows.Forms.Button();
			this.button10 = new System.Windows.Forms.Button();
			this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
			this.label3 = new System.Windows.Forms.Label();
			this.labelMonthlyIncomeSum = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.DatePicker = new System.Windows.Forms.DateTimePicker();
			this.tbLog = new System.Windows.Forms.RichTextBox();
			this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			this.button1 = new System.Windows.Forms.Button();
			this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
			label5 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label6 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			label8 = new System.Windows.Forms.Label();
			label9 = new System.Windows.Forms.Label();
			label14 = new System.Windows.Forms.Label();
			label15 = new System.Windows.Forms.Label();
			label11 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1.SuspendLayout();
			this.mainTabControl.SuspendLayout();
			this.tabDailyExpenses.SuspendLayout();
			this.panel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dailyExpensesDgv)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.DailyExpensesBindingSource)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel2.SuspendLayout();
			this.tableLayoutPanel3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.UnitBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.CategoryBindingSource)).BeginInit();
			this.flowLayoutPanel6.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.tabMonthlyExpenses.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.monthlyExpensesDgv)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MonthlyExpensesBindingSource)).BeginInit();
			this.flowLayoutPanel3.SuspendLayout();
			this.tabMonthlyIncomes.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.monthlyIncomesDgv)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MonthlyIncomesBindingSource)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.tableLayoutPanel4.SuspendLayout();
			this.tableLayoutPanel5.SuspendLayout();
			this.flowLayoutPanel7.SuspendLayout();
			this.flowLayoutPanel5.SuspendLayout();
			this.panel1.SuspendLayout();
			this.flowLayoutPanel2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
			this.SuspendLayout();
			// 
			// label5
			// 
			label5.AutoSize = true;
			label5.Dock = System.Windows.Forms.DockStyle.Fill;
			label5.Location = new System.Drawing.Point(3, 26);
			label5.Name = "label5";
			label5.Size = new System.Drawing.Size(104, 26);
			label5.TabIndex = 32;
			label5.Text = "Összeg:";
			label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			label4.AutoSize = true;
			label4.Dock = System.Windows.Forms.DockStyle.Fill;
			label4.Location = new System.Drawing.Point(3, 0);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(104, 26);
			label4.TabIndex = 31;
			label4.Text = "Megnevezés:";
			label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			label6.AutoSize = true;
			label6.Dock = System.Windows.Forms.DockStyle.Fill;
			label6.Location = new System.Drawing.Point(3, 52);
			label6.Name = "label6";
			label6.Size = new System.Drawing.Size(104, 27);
			label6.TabIndex = 34;
			label6.Text = "Kategória:";
			label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Dock = System.Windows.Forms.DockStyle.Fill;
			label7.Location = new System.Drawing.Point(3, 79);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(104, 27);
			label7.TabIndex = 40;
			label7.Text = "Mértékegység:";
			label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			label8.AutoSize = true;
			label8.Dock = System.Windows.Forms.DockStyle.Fill;
			label8.Location = new System.Drawing.Point(3, 106);
			label8.Name = "label8";
			label8.Size = new System.Drawing.Size(104, 26);
			label8.TabIndex = 36;
			label8.Text = "Megjegyzés:";
			label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			label9.AutoSize = true;
			label9.Dock = System.Windows.Forms.DockStyle.Fill;
			label9.Location = new System.Drawing.Point(3, 132);
			label9.Name = "label9";
			label9.Size = new System.Drawing.Size(104, 26);
			label9.TabIndex = 38;
			label9.Text = "Mennyiség:";
			label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label14
			// 
			label14.AutoSize = true;
			label14.Dock = System.Windows.Forms.DockStyle.Fill;
			label14.Location = new System.Drawing.Point(3, 0);
			label14.Name = "label14";
			label14.Size = new System.Drawing.Size(104, 26);
			label14.TabIndex = 31;
			label14.Text = "Megnevezés:";
			label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label15
			// 
			label15.AutoSize = true;
			label15.Dock = System.Windows.Forms.DockStyle.Fill;
			label15.Location = new System.Drawing.Point(3, 26);
			label15.Name = "label15";
			label15.Size = new System.Drawing.Size(104, 26);
			label15.TabIndex = 32;
			label15.Text = "Összeg:";
			label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			label11.AutoSize = true;
			label11.Dock = System.Windows.Forms.DockStyle.Fill;
			label11.Location = new System.Drawing.Point(3, 52);
			label11.Name = "label11";
			label11.Size = new System.Drawing.Size(104, 27);
			label11.TabIndex = 36;
			label11.Text = "Megjegyzés:";
			label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 34F));
			this.tableLayoutPanel1.Controls.Add(this.mainTabControl, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(760, 537);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// mainTabControl
			// 
			this.mainTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mainTabControl.Controls.Add(this.tabDailyExpenses);
			this.mainTabControl.Controls.Add(this.tabMonthlyExpenses);
			this.mainTabControl.Controls.Add(this.tabMonthlyIncomes);
			this.mainTabControl.Location = new System.Drawing.Point(3, 3);
			this.mainTabControl.Name = "mainTabControl";
			this.mainTabControl.SelectedIndex = 0;
			this.mainTabControl.Size = new System.Drawing.Size(495, 531);
			this.mainTabControl.TabIndex = 0;
			this.mainTabControl.SelectedIndexChanged += new System.EventHandler(this.MainTabControl_SelectedIndexChanged);
			// 
			// tabDailyExpenses
			// 
			this.tabDailyExpenses.Controls.Add(this.panel2);
			this.tabDailyExpenses.Location = new System.Drawing.Point(4, 22);
			this.tabDailyExpenses.Name = "tabDailyExpenses";
			this.tabDailyExpenses.Padding = new System.Windows.Forms.Padding(3);
			this.tabDailyExpenses.Size = new System.Drawing.Size(487, 505);
			this.tabDailyExpenses.TabIndex = 0;
			this.tabDailyExpenses.Text = "Napi kiadások";
			this.tabDailyExpenses.UseVisualStyleBackColor = true;
			// 
			// panel2
			// 
			this.panel2.AutoScroll = true;
			this.panel2.Controls.Add(this.dailyExpensesDgv);
			this.panel2.Controls.Add(this.groupBox1);
			this.panel2.Controls.Add(this.flowLayoutPanel1);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 3);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(481, 499);
			this.panel2.TabIndex = 0;
			// 
			// dailyExpensesDgv
			// 
			this.dailyExpensesDgv.AllowUserToAddRows = false;
			this.dailyExpensesDgv.AllowUserToDeleteRows = false;
			this.dailyExpensesDgv.AllowUserToOrderColumns = true;
			this.dailyExpensesDgv.AllowUserToResizeRows = false;
			dataGridViewCellStyle14.BackColor = System.Drawing.Color.Gainsboro;
			this.dailyExpensesDgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle14;
			this.dailyExpensesDgv.AutoGenerateColumns = false;
			this.dailyExpensesDgv.BackgroundColor = System.Drawing.SystemColors.Window;
			this.dailyExpensesDgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.dailyExpensesDgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.dailyExpensesDgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.dailyExpensesDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dailyExpensesDgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TitleDailyColumn,
            this.AmountDailyColumn,
            this.QuantityDailyColumn,
            this.CategoryDailyColumn,
            this.CommentDailyColumn});
			this.dailyExpensesDgv.DataSource = this.DailyExpensesBindingSource;
			this.dailyExpensesDgv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dailyExpensesDgv.Location = new System.Drawing.Point(0, 36);
			this.dailyExpensesDgv.Margin = new System.Windows.Forms.Padding(10);
			this.dailyExpensesDgv.Name = "dailyExpensesDgv";
			this.dailyExpensesDgv.ReadOnly = true;
			this.dailyExpensesDgv.RowHeadersVisible = false;
			this.dailyExpensesDgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dailyExpensesDgv.Size = new System.Drawing.Size(481, 246);
			this.dailyExpensesDgv.StandardTab = true;
			this.dailyExpensesDgv.TabIndex = 31;
			// 
			// TitleDailyColumn
			// 
			this.TitleDailyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.TitleDailyColumn.DataPropertyName = "Title";
			this.TitleDailyColumn.HeaderText = "Megnevezés";
			this.TitleDailyColumn.Name = "TitleDailyColumn";
			this.TitleDailyColumn.ReadOnly = true;
			this.TitleDailyColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.TitleDailyColumn.Width = 93;
			// 
			// AmountDailyColumn
			// 
			this.AmountDailyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.AmountDailyColumn.DataPropertyName = "AmountView";
			this.AmountDailyColumn.HeaderText = "Összeg";
			this.AmountDailyColumn.Name = "AmountDailyColumn";
			this.AmountDailyColumn.ReadOnly = true;
			this.AmountDailyColumn.Width = 67;
			// 
			// QuantityDailyColumn
			// 
			this.QuantityDailyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.QuantityDailyColumn.DataPropertyName = "QuantityView";
			this.QuantityDailyColumn.HeaderText = "Mennyiség";
			this.QuantityDailyColumn.Name = "QuantityDailyColumn";
			this.QuantityDailyColumn.ReadOnly = true;
			this.QuantityDailyColumn.Width = 83;
			// 
			// CategoryDailyColumn
			// 
			this.CategoryDailyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.CategoryDailyColumn.DataPropertyName = "CategoryView";
			this.CategoryDailyColumn.HeaderText = "Kategória";
			this.CategoryDailyColumn.Name = "CategoryDailyColumn";
			this.CategoryDailyColumn.ReadOnly = true;
			this.CategoryDailyColumn.Width = 77;
			// 
			// CommentDailyColumn
			// 
			this.CommentDailyColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.CommentDailyColumn.DataPropertyName = "Comment";
			this.CommentDailyColumn.HeaderText = "Megjegyzés";
			this.CommentDailyColumn.Name = "CommentDailyColumn";
			this.CommentDailyColumn.ReadOnly = true;
			this.CommentDailyColumn.Width = 88;
			// 
			// DailyExpensesBindingSource
			// 
			this.DailyExpensesBindingSource.DataSource = typeof(Common.UiModels.WinForms.ExpenseItemDgvViewModel);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.tableLayoutPanel2);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox1.Location = new System.Drawing.Point(0, 282);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(481, 217);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Új mai kiadás";
			// 
			// tableLayoutPanel2
			// 
			this.tableLayoutPanel2.ColumnCount = 1;
			this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.Controls.Add(this.tableLayoutPanel3, 0, 0);
			this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel6, 0, 1);
			this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel2.Name = "tableLayoutPanel2";
			this.tableLayoutPanel2.RowCount = 2;
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tableLayoutPanel2.Size = new System.Drawing.Size(475, 198);
			this.tableLayoutPanel2.TabIndex = 0;
			// 
			// tableLayoutPanel3
			// 
			this.tableLayoutPanel3.ColumnCount = 3;
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel3.Controls.Add(label7, 0, 3);
			this.tableLayoutPanel3.Controls.Add(this.cbExpenseUnit, 1, 3);
			this.tableLayoutPanel3.Controls.Add(label8, 0, 4);
			this.tableLayoutPanel3.Controls.Add(this.tbExpenseComment, 1, 4);
			this.tableLayoutPanel3.Controls.Add(label9, 0, 5);
			this.tableLayoutPanel3.Controls.Add(this.tbExpenseQuantity, 1, 5);
			this.tableLayoutPanel3.Controls.Add(label6, 0, 2);
			this.tableLayoutPanel3.Controls.Add(this.cbExpenseCategory, 1, 2);
			this.tableLayoutPanel3.Controls.Add(this.tbExpenseAmount, 1, 1);
			this.tableLayoutPanel3.Controls.Add(this.tbExpenseTitle, 1, 0);
			this.tableLayoutPanel3.Controls.Add(label4, 0, 0);
			this.tableLayoutPanel3.Controls.Add(label5, 0, 1);
			this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel3.Name = "tableLayoutPanel3";
			this.tableLayoutPanel3.RowCount = 6;
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel3.Size = new System.Drawing.Size(469, 157);
			this.tableLayoutPanel3.TabIndex = 0;
			// 
			// cbExpenseUnit
			// 
			this.cbExpenseUnit.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.DailyExpensesBindingSource, "Unit", true));
			this.cbExpenseUnit.DataSource = this.UnitBindingSource;
			this.cbExpenseUnit.DisplayMember = "DisplayName";
			this.cbExpenseUnit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbExpenseUnit.FormattingEnabled = true;
			this.cbExpenseUnit.Location = new System.Drawing.Point(113, 82);
			this.cbExpenseUnit.Name = "cbExpenseUnit";
			this.cbExpenseUnit.Size = new System.Drawing.Size(333, 21);
			this.cbExpenseUnit.TabIndex = 35;
			// 
			// UnitBindingSource
			// 
			this.UnitBindingSource.DataSource = typeof(Common.DbEntities.Unit);
			// 
			// tbExpenseComment
			// 
			this.tbExpenseComment.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.DailyExpensesBindingSource, "Comment", true));
			this.tbExpenseComment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbExpenseComment.Location = new System.Drawing.Point(113, 109);
			this.tbExpenseComment.Name = "tbExpenseComment";
			this.tbExpenseComment.Size = new System.Drawing.Size(333, 20);
			this.tbExpenseComment.TabIndex = 36;
			// 
			// tbExpenseQuantity
			// 
			this.tbExpenseQuantity.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.DailyExpensesBindingSource, "Quantity", true));
			this.tbExpenseQuantity.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbExpenseQuantity.Location = new System.Drawing.Point(113, 135);
			this.tbExpenseQuantity.Name = "tbExpenseQuantity";
			this.tbExpenseQuantity.Size = new System.Drawing.Size(333, 20);
			this.tbExpenseQuantity.TabIndex = 37;
			this.tbExpenseQuantity.Validating += new System.ComponentModel.CancelEventHandler(this.tbExpenseQuantity_Validating);
			// 
			// cbExpenseCategory
			// 
			this.cbExpenseCategory.DataBindings.Add(new System.Windows.Forms.Binding("SelectedItem", this.DailyExpensesBindingSource, "Category", true));
			this.cbExpenseCategory.DataSource = this.CategoryBindingSource;
			this.cbExpenseCategory.DisplayMember = "DisplayName";
			this.cbExpenseCategory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cbExpenseCategory.FormattingEnabled = true;
			this.cbExpenseCategory.Location = new System.Drawing.Point(113, 55);
			this.cbExpenseCategory.Name = "cbExpenseCategory";
			this.cbExpenseCategory.Size = new System.Drawing.Size(333, 21);
			this.cbExpenseCategory.TabIndex = 34;
			// 
			// CategoryBindingSource
			// 
			this.CategoryBindingSource.DataSource = typeof(Common.DbEntities.Category);
			// 
			// tbExpenseAmount
			// 
			this.tbExpenseAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.DailyExpensesBindingSource, "Amount", true));
			this.tbExpenseAmount.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbExpenseAmount.Location = new System.Drawing.Point(113, 29);
			this.tbExpenseAmount.Name = "tbExpenseAmount";
			this.tbExpenseAmount.Size = new System.Drawing.Size(333, 20);
			this.tbExpenseAmount.TabIndex = 33;
			this.tbExpenseAmount.Validating += new System.ComponentModel.CancelEventHandler(this.tbExpenseAmount_Validating);
			// 
			// tbExpenseTitle
			// 
			this.tbExpenseTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.DailyExpensesBindingSource, "Title", true));
			this.tbExpenseTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbExpenseTitle.Location = new System.Drawing.Point(113, 3);
			this.tbExpenseTitle.Name = "tbExpenseTitle";
			this.tbExpenseTitle.Size = new System.Drawing.Size(333, 20);
			this.tbExpenseTitle.TabIndex = 32;
			this.tbExpenseTitle.Validating += new System.ComponentModel.CancelEventHandler(this.tbExpenseTitle_Validating);
			// 
			// flowLayoutPanel6
			// 
			this.flowLayoutPanel6.Controls.Add(this.button3);
			this.flowLayoutPanel6.Controls.Add(this.button5);
			this.flowLayoutPanel6.Controls.Add(this.button6);
			this.flowLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel6.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel6.Location = new System.Drawing.Point(3, 166);
			this.flowLayoutPanel6.Name = "flowLayoutPanel6";
			this.flowLayoutPanel6.Size = new System.Drawing.Size(469, 29);
			this.flowLayoutPanel6.TabIndex = 1;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(391, 3);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(75, 23);
			this.button3.TabIndex = 41;
			this.button3.Text = "Mentés";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.SaveDailyExpensesButton_Click);
			// 
			// button5
			// 
			this.button5.Location = new System.Drawing.Point(310, 3);
			this.button5.Name = "button5";
			this.button5.Size = new System.Drawing.Size(75, 23);
			this.button5.TabIndex = 39;
			this.button5.Text = "Törlés";
			this.button5.UseVisualStyleBackColor = true;
			this.button5.Click += new System.EventHandler(this.RemoveExpenseButton_Click);
			// 
			// button6
			// 
			this.button6.Location = new System.Drawing.Point(229, 3);
			this.button6.Name = "button6";
			this.button6.Size = new System.Drawing.Size(75, 23);
			this.button6.TabIndex = 38;
			this.button6.Text = "Új";
			this.button6.UseVisualStyleBackColor = true;
			this.button6.Click += new System.EventHandler(this.NewExpenseButton_Click);
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.label1);
			this.flowLayoutPanel1.Controls.Add(this.labelDailySummary);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
			this.flowLayoutPanel1.Size = new System.Drawing.Size(481, 36);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 10);
			this.label1.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(68, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Napi összeg:";
			// 
			// labelDailySummary
			// 
			this.labelDailySummary.AutoSize = true;
			this.labelDailySummary.Location = new System.Drawing.Point(81, 10);
			this.labelDailySummary.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
			this.labelDailySummary.Name = "labelDailySummary";
			this.labelDailySummary.Size = new System.Drawing.Size(16, 13);
			this.labelDailySummary.TabIndex = 1;
			this.labelDailySummary.Text = "...";
			// 
			// tabMonthlyExpenses
			// 
			this.tabMonthlyExpenses.Controls.Add(this.monthlyExpensesDgv);
			this.tabMonthlyExpenses.Controls.Add(this.flowLayoutPanel3);
			this.tabMonthlyExpenses.Location = new System.Drawing.Point(4, 22);
			this.tabMonthlyExpenses.Name = "tabMonthlyExpenses";
			this.tabMonthlyExpenses.Padding = new System.Windows.Forms.Padding(3);
			this.tabMonthlyExpenses.Size = new System.Drawing.Size(487, 505);
			this.tabMonthlyExpenses.TabIndex = 1;
			this.tabMonthlyExpenses.Text = "Havi kiadások";
			this.tabMonthlyExpenses.UseVisualStyleBackColor = true;
			// 
			// monthlyExpensesDgv
			// 
			this.monthlyExpensesDgv.AllowUserToAddRows = false;
			this.monthlyExpensesDgv.AllowUserToDeleteRows = false;
			this.monthlyExpensesDgv.AllowUserToOrderColumns = true;
			this.monthlyExpensesDgv.AllowUserToResizeRows = false;
			dataGridViewCellStyle15.BackColor = System.Drawing.Color.Gainsboro;
			this.monthlyExpensesDgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle15;
			this.monthlyExpensesDgv.AutoGenerateColumns = false;
			this.monthlyExpensesDgv.BackgroundColor = System.Drawing.SystemColors.Window;
			this.monthlyExpensesDgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.monthlyExpensesDgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.monthlyExpensesDgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.monthlyExpensesDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.monthlyExpensesDgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Date,
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5});
			this.monthlyExpensesDgv.DataSource = this.MonthlyExpensesBindingSource;
			this.monthlyExpensesDgv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monthlyExpensesDgv.Location = new System.Drawing.Point(3, 39);
			this.monthlyExpensesDgv.Margin = new System.Windows.Forms.Padding(10);
			this.monthlyExpensesDgv.Name = "monthlyExpensesDgv";
			this.monthlyExpensesDgv.ReadOnly = true;
			this.monthlyExpensesDgv.RowHeadersVisible = false;
			this.monthlyExpensesDgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.monthlyExpensesDgv.Size = new System.Drawing.Size(481, 463);
			this.monthlyExpensesDgv.StandardTab = true;
			this.monthlyExpensesDgv.TabIndex = 32;
			// 
			// Date
			// 
			this.Date.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.Date.DataPropertyName = "Date";
			dataGridViewCellStyle16.Format = "dd";
			dataGridViewCellStyle16.NullValue = null;
			this.Date.DefaultCellStyle = dataGridViewCellStyle16;
			this.Date.HeaderText = "Nap";
			this.Date.Name = "Date";
			this.Date.ReadOnly = true;
			this.Date.Width = 52;
			// 
			// dataGridViewTextBoxColumn1
			// 
			this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.dataGridViewTextBoxColumn1.DataPropertyName = "Title";
			this.dataGridViewTextBoxColumn1.HeaderText = "Megnevezés";
			this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
			this.dataGridViewTextBoxColumn1.ReadOnly = true;
			this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewTextBoxColumn1.Width = 93;
			// 
			// dataGridViewTextBoxColumn2
			// 
			this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.dataGridViewTextBoxColumn2.DataPropertyName = "AmountView";
			this.dataGridViewTextBoxColumn2.HeaderText = "Összeg";
			this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
			this.dataGridViewTextBoxColumn2.ReadOnly = true;
			this.dataGridViewTextBoxColumn2.Width = 67;
			// 
			// dataGridViewTextBoxColumn3
			// 
			this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.dataGridViewTextBoxColumn3.DataPropertyName = "QuantityView";
			this.dataGridViewTextBoxColumn3.HeaderText = "Mennyiség";
			this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
			this.dataGridViewTextBoxColumn3.ReadOnly = true;
			this.dataGridViewTextBoxColumn3.Width = 83;
			// 
			// dataGridViewTextBoxColumn4
			// 
			this.dataGridViewTextBoxColumn4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.dataGridViewTextBoxColumn4.DataPropertyName = "CategoryView";
			this.dataGridViewTextBoxColumn4.HeaderText = "Kategória";
			this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
			this.dataGridViewTextBoxColumn4.ReadOnly = true;
			this.dataGridViewTextBoxColumn4.Width = 77;
			// 
			// dataGridViewTextBoxColumn5
			// 
			this.dataGridViewTextBoxColumn5.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.dataGridViewTextBoxColumn5.DataPropertyName = "Comment";
			this.dataGridViewTextBoxColumn5.HeaderText = "Megjegyzés";
			this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
			this.dataGridViewTextBoxColumn5.ReadOnly = true;
			this.dataGridViewTextBoxColumn5.Width = 88;
			// 
			// MonthlyExpensesBindingSource
			// 
			this.MonthlyExpensesBindingSource.DataSource = typeof(Common.UiModels.WinForms.ExpenseItemDgvViewModel);
			// 
			// flowLayoutPanel3
			// 
			this.flowLayoutPanel3.Controls.Add(this.label2);
			this.flowLayoutPanel3.Controls.Add(this.labelMonthlySummary);
			this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel3.Location = new System.Drawing.Point(3, 3);
			this.flowLayoutPanel3.Name = "flowLayoutPanel3";
			this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(10);
			this.flowLayoutPanel3.Size = new System.Drawing.Size(481, 36);
			this.flowLayoutPanel3.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(68, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Havi összeg:";
			// 
			// labelMonthlySummary
			// 
			this.labelMonthlySummary.AutoSize = true;
			this.labelMonthlySummary.Location = new System.Drawing.Point(87, 10);
			this.labelMonthlySummary.Name = "labelMonthlySummary";
			this.labelMonthlySummary.Size = new System.Drawing.Size(16, 13);
			this.labelMonthlySummary.TabIndex = 1;
			this.labelMonthlySummary.Text = "...";
			// 
			// tabMonthlyIncomes
			// 
			this.tabMonthlyIncomes.AutoScroll = true;
			this.tabMonthlyIncomes.Controls.Add(this.monthlyIncomesDgv);
			this.tabMonthlyIncomes.Controls.Add(this.groupBox2);
			this.tabMonthlyIncomes.Controls.Add(this.flowLayoutPanel5);
			this.tabMonthlyIncomes.Location = new System.Drawing.Point(4, 22);
			this.tabMonthlyIncomes.Name = "tabMonthlyIncomes";
			this.tabMonthlyIncomes.Padding = new System.Windows.Forms.Padding(3);
			this.tabMonthlyIncomes.Size = new System.Drawing.Size(487, 505);
			this.tabMonthlyIncomes.TabIndex = 2;
			this.tabMonthlyIncomes.Text = "Havi bevételek";
			this.tabMonthlyIncomes.UseVisualStyleBackColor = true;
			// 
			// monthlyIncomesDgv
			// 
			this.monthlyIncomesDgv.AllowUserToAddRows = false;
			this.monthlyIncomesDgv.AllowUserToDeleteRows = false;
			this.monthlyIncomesDgv.AllowUserToOrderColumns = true;
			this.monthlyIncomesDgv.AllowUserToResizeRows = false;
			dataGridViewCellStyle13.BackColor = System.Drawing.Color.Gainsboro;
			this.monthlyIncomesDgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle13;
			this.monthlyIncomesDgv.AutoGenerateColumns = false;
			this.monthlyIncomesDgv.BackgroundColor = System.Drawing.SystemColors.Window;
			this.monthlyIncomesDgv.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.monthlyIncomesDgv.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
			this.monthlyIncomesDgv.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.monthlyIncomesDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.monthlyIncomesDgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn6,
            this.dataGridViewTextBoxColumn7,
            this.dataGridViewTextBoxColumn10});
			this.monthlyIncomesDgv.DataSource = this.MonthlyIncomesBindingSource;
			this.monthlyIncomesDgv.Dock = System.Windows.Forms.DockStyle.Fill;
			this.monthlyIncomesDgv.Location = new System.Drawing.Point(3, 39);
			this.monthlyIncomesDgv.Margin = new System.Windows.Forms.Padding(10);
			this.monthlyIncomesDgv.Name = "monthlyIncomesDgv";
			this.monthlyIncomesDgv.ReadOnly = true;
			this.monthlyIncomesDgv.RowHeadersVisible = false;
			this.monthlyIncomesDgv.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.monthlyIncomesDgv.Size = new System.Drawing.Size(481, 324);
			this.monthlyIncomesDgv.StandardTab = true;
			this.monthlyIncomesDgv.TabIndex = 33;
			// 
			// dataGridViewTextBoxColumn6
			// 
			this.dataGridViewTextBoxColumn6.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.dataGridViewTextBoxColumn6.DataPropertyName = "Title";
			this.dataGridViewTextBoxColumn6.HeaderText = "Megnevezés";
			this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
			this.dataGridViewTextBoxColumn6.ReadOnly = true;
			this.dataGridViewTextBoxColumn6.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.dataGridViewTextBoxColumn6.Width = 93;
			// 
			// dataGridViewTextBoxColumn7
			// 
			this.dataGridViewTextBoxColumn7.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
			this.dataGridViewTextBoxColumn7.DataPropertyName = "AmountView";
			this.dataGridViewTextBoxColumn7.HeaderText = "Összeg";
			this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
			this.dataGridViewTextBoxColumn7.ReadOnly = true;
			this.dataGridViewTextBoxColumn7.Width = 67;
			// 
			// dataGridViewTextBoxColumn10
			// 
			this.dataGridViewTextBoxColumn10.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.dataGridViewTextBoxColumn10.DataPropertyName = "Comment";
			this.dataGridViewTextBoxColumn10.HeaderText = "Megjegyzés";
			this.dataGridViewTextBoxColumn10.Name = "dataGridViewTextBoxColumn10";
			this.dataGridViewTextBoxColumn10.ReadOnly = true;
			this.dataGridViewTextBoxColumn10.Width = 88;
			// 
			// MonthlyIncomesBindingSource
			// 
			this.MonthlyIncomesBindingSource.DataSource = typeof(Common.UiModels.WinForms.ExpenseItemDgvViewModel);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.tableLayoutPanel4);
			this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.groupBox2.Location = new System.Drawing.Point(3, 363);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(481, 139);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Új ehavi kiadás";
			// 
			// tableLayoutPanel4
			// 
			this.tableLayoutPanel4.ColumnCount = 1;
			this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.Controls.Add(this.tableLayoutPanel5, 0, 0);
			this.tableLayoutPanel4.Controls.Add(this.flowLayoutPanel7, 0, 1);
			this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 16);
			this.tableLayoutPanel4.Name = "tableLayoutPanel4";
			this.tableLayoutPanel4.RowCount = 2;
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.tableLayoutPanel4.Size = new System.Drawing.Size(475, 120);
			this.tableLayoutPanel4.TabIndex = 1;
			// 
			// tableLayoutPanel5
			// 
			this.tableLayoutPanel5.AutoScroll = true;
			this.tableLayoutPanel5.ColumnCount = 3;
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel5.Controls.Add(label14, 0, 0);
			this.tableLayoutPanel5.Controls.Add(label15, 0, 1);
			this.tableLayoutPanel5.Controls.Add(label11, 0, 2);
			this.tableLayoutPanel5.Controls.Add(this.tbIncomeTitle, 1, 0);
			this.tableLayoutPanel5.Controls.Add(this.tbIncomeAmount, 1, 1);
			this.tableLayoutPanel5.Controls.Add(this.tbIncomeComment, 1, 2);
			this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel5.Location = new System.Drawing.Point(3, 3);
			this.tableLayoutPanel5.Name = "tableLayoutPanel5";
			this.tableLayoutPanel5.RowCount = 3;
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.tableLayoutPanel5.Size = new System.Drawing.Size(469, 79);
			this.tableLayoutPanel5.TabIndex = 0;
			// 
			// tbIncomeTitle
			// 
			this.tbIncomeTitle.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.MonthlyIncomesBindingSource, "Title", true));
			this.tbIncomeTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbIncomeTitle.Location = new System.Drawing.Point(113, 3);
			this.tbIncomeTitle.Name = "tbIncomeTitle";
			this.tbIncomeTitle.Size = new System.Drawing.Size(333, 20);
			this.tbIncomeTitle.TabIndex = 41;
			this.tbIncomeTitle.Validating += new System.ComponentModel.CancelEventHandler(this.tbIncomeTitle_Validating);
			// 
			// tbIncomeAmount
			// 
			this.tbIncomeAmount.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.MonthlyIncomesBindingSource, "Amount", true));
			this.tbIncomeAmount.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbIncomeAmount.Location = new System.Drawing.Point(113, 29);
			this.tbIncomeAmount.Name = "tbIncomeAmount";
			this.tbIncomeAmount.Size = new System.Drawing.Size(333, 20);
			this.tbIncomeAmount.TabIndex = 42;
			this.tbIncomeAmount.Validating += new System.ComponentModel.CancelEventHandler(this.tbIncomeAmount_Validating);
			// 
			// tbIncomeComment
			// 
			this.tbIncomeComment.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.MonthlyIncomesBindingSource, "Comment", true));
			this.tbIncomeComment.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbIncomeComment.Location = new System.Drawing.Point(113, 55);
			this.tbIncomeComment.Name = "tbIncomeComment";
			this.tbIncomeComment.Size = new System.Drawing.Size(333, 20);
			this.tbIncomeComment.TabIndex = 43;
			// 
			// flowLayoutPanel7
			// 
			this.flowLayoutPanel7.Controls.Add(this.button7);
			this.flowLayoutPanel7.Controls.Add(this.button9);
			this.flowLayoutPanel7.Controls.Add(this.button10);
			this.flowLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel7.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel7.Location = new System.Drawing.Point(3, 88);
			this.flowLayoutPanel7.Name = "flowLayoutPanel7";
			this.flowLayoutPanel7.Size = new System.Drawing.Size(469, 29);
			this.flowLayoutPanel7.TabIndex = 1;
			// 
			// button7
			// 
			this.button7.Location = new System.Drawing.Point(391, 3);
			this.button7.Name = "button7";
			this.button7.Size = new System.Drawing.Size(75, 23);
			this.button7.TabIndex = 0;
			this.button7.Text = "Mentés";
			this.button7.UseVisualStyleBackColor = true;
			this.button7.Click += new System.EventHandler(this.SaveMonthlyIncomesButton_Click);
			// 
			// button9
			// 
			this.button9.Location = new System.Drawing.Point(310, 3);
			this.button9.Name = "button9";
			this.button9.Size = new System.Drawing.Size(75, 23);
			this.button9.TabIndex = 2;
			this.button9.Text = "Törlés";
			this.button9.UseVisualStyleBackColor = true;
			this.button9.Click += new System.EventHandler(this.RemoveIncomeButton_Click);
			// 
			// button10
			// 
			this.button10.Location = new System.Drawing.Point(229, 3);
			this.button10.Name = "button10";
			this.button10.Size = new System.Drawing.Size(75, 23);
			this.button10.TabIndex = 3;
			this.button10.Text = "Új";
			this.button10.UseVisualStyleBackColor = true;
			this.button10.Click += new System.EventHandler(this.NewIncomeButton_Click);
			// 
			// flowLayoutPanel5
			// 
			this.flowLayoutPanel5.Controls.Add(this.label3);
			this.flowLayoutPanel5.Controls.Add(this.labelMonthlyIncomeSum);
			this.flowLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Top;
			this.flowLayoutPanel5.Location = new System.Drawing.Point(3, 3);
			this.flowLayoutPanel5.Name = "flowLayoutPanel5";
			this.flowLayoutPanel5.Padding = new System.Windows.Forms.Padding(10);
			this.flowLayoutPanel5.Size = new System.Drawing.Size(481, 36);
			this.flowLayoutPanel5.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(13, 10);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Havi bevétel összeg:";
			// 
			// labelMonthlyIncomeSum
			// 
			this.labelMonthlyIncomeSum.AutoSize = true;
			this.labelMonthlyIncomeSum.Location = new System.Drawing.Point(125, 10);
			this.labelMonthlyIncomeSum.Name = "labelMonthlyIncomeSum";
			this.labelMonthlyIncomeSum.Size = new System.Drawing.Size(16, 13);
			this.labelMonthlyIncomeSum.TabIndex = 1;
			this.labelMonthlyIncomeSum.Text = "...";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.DatePicker);
			this.panel1.Controls.Add(this.tbLog);
			this.panel1.Controls.Add(this.flowLayoutPanel2);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(504, 3);
			this.panel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(253, 534);
			this.panel1.TabIndex = 1;
			// 
			// DatePicker
			// 
			this.DatePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.DatePicker.Location = new System.Drawing.Point(5, 7);
			this.DatePicker.Name = "DatePicker";
			this.DatePicker.Size = new System.Drawing.Size(242, 20);
			this.DatePicker.TabIndex = 4;
			this.DatePicker.ValueChanged += new System.EventHandler(this.DatePicker_ValueChanged);
			// 
			// tbLog
			// 
			this.tbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbLog.Location = new System.Drawing.Point(5, 33);
			this.tbLog.Name = "tbLog";
			this.tbLog.Size = new System.Drawing.Size(242, 455);
			this.tbLog.TabIndex = 5;
			this.tbLog.Text = "";
			// 
			// flowLayoutPanel2
			// 
			this.flowLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel2.Controls.Add(this.button1);
			this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel2.Location = new System.Drawing.Point(7, 494);
			this.flowLayoutPanel2.Margin = new System.Windows.Forms.Padding(0);
			this.flowLayoutPanel2.Name = "flowLayoutPanel2";
			this.flowLayoutPanel2.Size = new System.Drawing.Size(240, 30);
			this.flowLayoutPanel2.TabIndex = 6;
			// 
			// button1
			// 
			this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.button1.Location = new System.Drawing.Point(162, 3);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(75, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Törlés";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.ClearLogButton_OnClick);
			// 
			// errorProvider1
			// 
			this.errorProvider1.ContainerControl = this;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.ClientSize = new System.Drawing.Size(784, 561);
			this.Controls.Add(this.tableLayoutPanel1);
			this.MinimumSize = new System.Drawing.Size(640, 480);
			this.Name = "Form1";
			this.Text = "Form1";
			this.tableLayoutPanel1.ResumeLayout(false);
			this.mainTabControl.ResumeLayout(false);
			this.tabDailyExpenses.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.dailyExpensesDgv)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.DailyExpensesBindingSource)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.tableLayoutPanel2.ResumeLayout(false);
			this.tableLayoutPanel3.ResumeLayout(false);
			this.tableLayoutPanel3.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.UnitBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.CategoryBindingSource)).EndInit();
			this.flowLayoutPanel6.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.tabMonthlyExpenses.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.monthlyExpensesDgv)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MonthlyExpensesBindingSource)).EndInit();
			this.flowLayoutPanel3.ResumeLayout(false);
			this.flowLayoutPanel3.PerformLayout();
			this.tabMonthlyIncomes.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.monthlyIncomesDgv)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MonthlyIncomesBindingSource)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.tableLayoutPanel4.ResumeLayout(false);
			this.tableLayoutPanel5.ResumeLayout(false);
			this.tableLayoutPanel5.PerformLayout();
			this.flowLayoutPanel7.ResumeLayout(false);
			this.flowLayoutPanel5.ResumeLayout(false);
			this.flowLayoutPanel5.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.flowLayoutPanel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.TabControl mainTabControl;
		private System.Windows.Forms.TabPage tabDailyExpenses;
		private System.Windows.Forms.TabPage tabMonthlyExpenses;
		private System.Windows.Forms.TabPage tabMonthlyIncomes;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.DateTimePicker DatePicker;
		private System.Windows.Forms.RichTextBox tbLog;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelDailySummary;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelMonthlySummary;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelMonthlyIncomeSum;
		private System.Windows.Forms.BindingSource DailyExpensesBindingSource;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
		private System.Windows.Forms.TextBox tbExpenseTitle;
		private System.Windows.Forms.TextBox tbExpenseAmount;
		private System.Windows.Forms.ComboBox cbExpenseUnit;
		private System.Windows.Forms.TextBox tbExpenseComment;
		private System.Windows.Forms.TextBox tbExpenseQuantity;
		private System.Windows.Forms.ComboBox cbExpenseCategory;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button5;
		private System.Windows.Forms.Button button6;
		private System.Windows.Forms.DataGridView dailyExpensesDgv;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel7;
		private System.Windows.Forms.Button button7;
		private System.Windows.Forms.Button button9;
		private System.Windows.Forms.Button button10;
		private System.Windows.Forms.DataGridViewTextBoxColumn amountDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn quantityDataGridViewTextBoxColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn categoryDataGridViewTextBoxColumn;
		private System.Windows.Forms.BindingSource CategoryBindingSource;
		private System.Windows.Forms.BindingSource UnitBindingSource;
		private System.Windows.Forms.DataGridViewTextBoxColumn TitleDailyColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn AmountDailyColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn QuantityDailyColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn CategoryDailyColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn CommentDailyColumn;
		private System.Windows.Forms.DataGridView monthlyExpensesDgv;
		private System.Windows.Forms.BindingSource MonthlyExpensesBindingSource;
		private System.Windows.Forms.DataGridView monthlyIncomesDgv;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn10;
		private System.Windows.Forms.BindingSource MonthlyIncomesBindingSource;
		private System.Windows.Forms.TextBox tbIncomeTitle;
		private System.Windows.Forms.TextBox tbIncomeAmount;
		private System.Windows.Forms.TextBox tbIncomeComment;
		private System.Windows.Forms.DataGridViewTextBoxColumn Date;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
		public System.Windows.Forms.ErrorProvider errorProvider1;
	}
}

