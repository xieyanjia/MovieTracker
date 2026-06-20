using System;
using System.Drawing;
using System.Windows.Forms;

namespace MovieTracker
{
    public partial class AddEditForm : Form
    {
        private Movie movie;
        private TextBox txtTitle, txtNotes, txtCover;
        private DateTimePicker dtpWatchDate;
        private CheckBox chkReminder;
        private ComboBox cmbType, cmbStatus, cmbGenre, cmbRating;
        private Button btnSave, btnCancel, btnBrowse;
        private PictureBox picCover;
        private Label lblPreview;

        public AddEditForm(Movie m)
        {
            movie = m;
            BuildUI();
            if (movie != null) FillForm();
        }

        private void BuildUI()
        {
            this.Text = movie == null ? "➕ 新增影片" : "✏️ 編輯影片";
            this.Size = new Size(500, 620);
            this.BackColor = Color.FromArgb(24, 24, 37);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;

            var lblH = new Label { Text = movie == null ? "新增影片" : "編輯影片", Font = new Font("Segoe UI", 14f, FontStyle.Bold), ForeColor = Color.FromArgb(203, 166, 247), Location = new Point(20, y), AutoSize = true }; y += 45;

            // Title
            AddLabel("影片標題 *", 20, y); y += 25;
            txtTitle = AddTextBox(20, y, 440); y += 40;

            // Type & Status
            AddLabel("類型", 20, y);
            AddLabel("狀態", 240, y); y += 25;
            cmbType = AddCombo(new[] { "電影", "影集" }, 20, y, 190);
            cmbStatus = AddCombo(new[] { "想看", "看過", "看一半" }, 240, y, 190); y += 40;

            // Genre & Rating
            AddLabel("風格", 20, y);
            var lblRating = new Label { Text = "評分", ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(240, y), AutoSize = true };
            this.Controls.Add(lblRating);
            y += 25;
            cmbGenre = AddCombo(new[] { "動作", "喜劇", "愛情", "恐怖", "科幻", "劇情", "動畫", "其他" }, 20, y, 190);
            cmbRating = AddCombo(new[] { "1分", "2分", "3分", "4分", "5分", "6分", "7分", "8分", "9分", "10分" }, 240, y, 190);
            y += 40;

            // Notes
            AddLabel("心得備註", 20, y); y += 25;
            txtNotes = new TextBox { Location = new Point(20, y), Size = new Size(440, 70), Multiline = true, BackColor = Color.FromArgb(40, 40, 60), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, ScrollBars = ScrollBars.Vertical };
            this.Controls.Add(txtNotes); y += 80;

            // Watch Date & Reminder
            var lblWatchDate = new Label { Text = "預計觀看日期", ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(20, y), AutoSize = true };
            var lblReminder = new Label { Text = "設定提醒", ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(240, y), AutoSize = true };
            this.Controls.AddRange(new Control[] { lblWatchDate, lblReminder });
            y += 25;
            dtpWatchDate = new DateTimePicker { Location = new Point(20, y), Width = 190, Format = DateTimePickerFormat.Short, BackColor = Color.FromArgb(40, 40, 60) };
            chkReminder = new CheckBox { Text = "提醒我看這部", Location = new Point(240, y), ForeColor = Color.White, AutoSize = true };
            this.Controls.AddRange(new Control[] { dtpWatchDate, chkReminder }); y += 40;

            cmbStatus.SelectedIndexChanged += (s, e) =>
            {
                bool show = cmbStatus.SelectedItem.ToString() != "看過";
                dtpWatchDate.Visible = show;
                chkReminder.Visible = show;
                lblWatchDate.Visible = show;
                lblReminder.Visible = show;

                bool showRating = cmbStatus.SelectedItem.ToString() != "想看";
                cmbRating.Visible = showRating;
                lblRating.Visible = showRating;
            };

            bool initShow = cmbStatus.SelectedItem.ToString() != "看過";
            dtpWatchDate.Visible = initShow;
            chkReminder.Visible = initShow;
            lblWatchDate.Visible = initShow;
            lblReminder.Visible = initShow;

            bool initShowRating = cmbStatus.SelectedItem.ToString() != "想看";
            cmbRating.Visible = initShowRating;
            lblRating.Visible = initShowRating;
            // Cover
            AddLabel("封面圖片路徑（選填）", 20, y); y += 25;
            txtCover = new TextBox { Location = new Point(20, y), Width = 340, BackColor = Color.FromArgb(40, 40, 60), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            btnBrowse = new Button { Text = "瀏覽", Location = new Point(370, y), Width = 90, Height = 26, BackColor = Color.FromArgb(137, 180, 250), ForeColor = Color.FromArgb(18, 18, 28), FlatStyle = FlatStyle.Flat, FlatAppearance = { BorderSize = 0 } };
            btnBrowse.Click += BtnBrowse_Click;
            txtCover.TextChanged += (s, e) => UpdatePreview();
            this.Controls.AddRange(new Control[] { txtCover, btnBrowse }); y += 40;

            // Buttons
            btnSave = new Button { Text = "💾  儲存", Location = new Point(20, y), Width = 200, Height = 40, BackColor = Color.FromArgb(166, 227, 161), ForeColor = Color.FromArgb(18, 18, 28), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11f, FontStyle.Bold), FlatAppearance = { BorderSize = 0 }, Cursor = Cursors.Hand };
            btnCancel = new Button { Text = "取消", Location = new Point(230, y), Width = 200, Height = 40, BackColor = Color.FromArgb(243, 139, 168), ForeColor = Color.FromArgb(18, 18, 28), FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 11f, FontStyle.Bold), FlatAppearance = { BorderSize = 0 }, Cursor = Cursors.Hand };

            btnSave.Click += BtnSave_Click;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.AddRange(new Control[] { lblH, btnSave, btnCancel });
        }

        private void AddLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label { Text = text, ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(x, y), AutoSize = true });
        }

        private TextBox AddTextBox(int x, int y, int width)
        {
            var tb = new TextBox { Location = new Point(x, y), Width = width, BackColor = Color.FromArgb(40, 40, 60), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            this.Controls.Add(tb);
            return tb;
        }

        private ComboBox AddCombo(string[] items, int x, int y, int width)
        {
            var cmb = new ComboBox { Location = new Point(x, y), Width = width, BackColor = Color.FromArgb(40, 40, 60), ForeColor = Color.White, DropDownStyle = ComboBoxStyle.DropDownList, FlatStyle = FlatStyle.Flat };
            cmb.Items.AddRange(items);
            cmb.SelectedIndex = 0;
            this.Controls.Add(cmb);
            return cmb;
        }

        private void FillForm()
        {
            txtTitle.Text = movie.Title;
            cmbType.SelectedItem = movie.Type;
            cmbStatus.SelectedItem = movie.Status;
            cmbGenre.SelectedItem = movie.Genre;
            cmbRating.SelectedIndex = movie.Rating > 0 ? movie.Rating - 1 : 0;
            txtNotes.Text = movie.Notes;
            txtCover.Text = movie.CoverUrl;

            if (!string.IsNullOrEmpty(movie.WatchDate) && DateTime.TryParse(movie.WatchDate, out DateTime wd))
                dtpWatchDate.Value = wd;
            chkReminder.Checked = movie.Reminder;
        }

        private void BtnBrowse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog { Filter = "圖片檔案|*.jpg;*.jpeg;*.png;*.bmp;*.gif" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                if (!System.IO.Directory.Exists(imagesDir))
                    System.IO.Directory.CreateDirectory(imagesDir);

                string fileName = System.IO.Path.GetFileName(ofd.FileName);
                string destPath = System.IO.Path.Combine(imagesDir, fileName);

                if (!System.IO.File.Exists(destPath))
                    System.IO.File.Copy(ofd.FileName, destPath);

                txtCover.Text = fileName; // 只存檔名
            }
        }

        private void UpdatePreview() { }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("請輸入影片標題！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var m = new Movie
            {
                WatchDate = dtpWatchDate.Value.ToString("yyyy-MM-dd"),
                Reminder = chkReminder.Checked,
                Title = txtTitle.Text.Trim(),
                Type = cmbType.SelectedItem.ToString(),
                Status = cmbStatus.SelectedItem.ToString(),
                Genre = cmbGenre.SelectedItem.ToString(),
                Rating = cmbStatus.SelectedItem.ToString() == "想看" ? 0 : cmbRating.SelectedIndex + 1,
                Notes = txtNotes.Text.Trim(),
                CoverUrl = txtCover.Text.Trim(),
                AddedDate = DateTime.Now.ToString("yyyy-MM-dd")
            };

            if (movie == null)
                Database.Add(m);
            else
            {
                m.Id = movie.Id;
                Database.Update(m);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}