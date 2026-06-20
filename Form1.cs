using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace MovieTracker
{
    public partial class Form1 : Form
    {
        private List<Movie> currentMovies = new List<Movie>();
       
        private PictureBox picDetail;
        private Label lblDetailTitle, lblDetailType, lblDetailStatus, lblDetailRating, lblDetailGenre, lblDetailNotes;
        
        // Controls
        private Panel topPanel, sidePanel, mainPanel;
        private TextBox searchBox;
        private ComboBox cmbType, cmbStatus, cmbGenre;
        private Button btnAdd, btnEdit, btnDelete, btnStats, btnSearch;
        private DataGridView grid;
        private Label lblTitle, lblCount;
        private Button btnExport;
        private Button btnTheme;
        private bool isDarkMode = true;

        public Form1()
        {
            InitializeComponent();
            Database.Initialize();
            BuildUI();
            LoadMovies();
            CheckReminders();
        }

        private void BuildUI()
        {
            this.Text = "🎬 Movie Tracker";
            this.Size = new Size(1100, 700);
            this.MinimumSize = new Size(900, 600);
            this.BackColor = Color.FromArgb(18, 18, 28);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Top Panel
            topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(30, 30, 46)
            };

            lblTitle = new Label
            {
                Text = "🎬  Movie Tracker",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(203, 166, 247),
                Location = new Point(20, 12),
                AutoSize = true
            };

            lblCount = new Label
            {
                Text = "0 部影片",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(166, 227, 161),
                AutoSize = true,
                Location = new Point(250, 20)
            };

            topPanel.Controls.AddRange(new Control[] { lblTitle, lblCount });

            // Side Panel
            sidePanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(24, 24, 37),
                Padding = new Padding(10)
            };

            int sy = 20;

            var lblFilter = new Label { Text = "篩選條件", ForeColor = Color.FromArgb(203, 166, 247), Font = new Font("Segoe UI", 11f, FontStyle.Bold), Location = new Point(10, sy), AutoSize = true };
            sy += 35;

            var lblSearch = new Label { Text = "搜尋標題", ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(10, sy), AutoSize = true };
            sy += 22;
            searchBox = new TextBox { Location = new Point(10, sy), Width = 190, BackColor = Color.FromArgb(40, 40, 60), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            sy += 35;

            var lblType = new Label { Text = "類型", ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(10, sy), AutoSize = true };
            sy += 22;
            cmbType = CreateCombo(new[] { "全部", "電影", "影集" }, sy); sy += 35;

            var lblStatus = new Label { Text = "狀態", ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(10, sy), AutoSize = true };
            sy += 22;
            cmbStatus = CreateCombo(new[] { "全部", "想看", "看過", "看一半" }, sy); sy += 35;

            var lblGenre = new Label { Text = "風格", ForeColor = Color.FromArgb(180, 180, 200), Location = new Point(10, sy), AutoSize = true };
            sy += 22;
            cmbGenre = CreateCombo(new[] { "全部", "動作", "喜劇", "愛情", "恐怖", "科幻", "劇情", "動畫", "其他" }, sy); sy += 40;

            btnSearch = CreateButton("🔍  搜尋", sy, Color.FromArgb(137, 180, 250)); sy += 45;
            btnAdd = CreateButton("➕  新增影片", sy, Color.FromArgb(166, 227, 161)); sy += 45;
            btnEdit = CreateButton("✏️  編輯", sy, Color.FromArgb(250, 179, 135)); sy += 45;
            btnDelete = CreateButton("🗑️  刪除", sy, Color.FromArgb(243, 139, 168)); sy += 45;
            btnStats = CreateButton("📊  統計圖表", sy, Color.FromArgb(203, 166, 247));
            sy += 45;
            btnExport = CreateButton("📤  匯出 CSV", sy, Color.FromArgb(137, 220, 235)); sy += 45;
            btnTheme = CreateButton("☀️  切換淺色主題", sy, Color.FromArgb(249, 226, 175));

            sidePanel.Controls.AddRange(new Control[] {
                lblFilter, lblSearch, searchBox,
                lblType, cmbType, lblStatus, cmbStatus,
                lblGenre, cmbGenre,
                btnSearch, btnAdd, btnEdit, btnDelete, btnStats, btnExport, btnTheme
            });

            // Main Panel - Grid
            mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(18, 18, 28), Padding = new Padding(10) };

            grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(24, 24, 37),
                BorderStyle = BorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.White,
                GridColor = Color.FromArgb(40, 40, 60),
                RowTemplate = { Height = 40 }
            };

            grid.DefaultCellStyle.BackColor = Color.FromArgb(24, 24, 37);
            grid.DefaultCellStyle.ForeColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(137, 180, 250);
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 46);
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(203, 166, 247);
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            grid.EnableHeadersVisualStyles = false;
            grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(30, 30, 46);

            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Visible = false });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Title", HeaderText = "📽️ 標題" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "類型" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "狀態" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Rating", HeaderText = "評分", ValueType = typeof(int) });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Genre", HeaderText = "風格" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Notes", HeaderText = "備註" });
            grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "AddedDate", HeaderText = "新增日期" });

            grid.Columns["Title"].FillWeight = 200;
            grid.Columns["Notes"].FillWeight = 150;

            mainPanel.Controls.Add(grid);

            // Detail Panel (右側預覽)
            var detailPanel = new Panel
            {
                Dock = DockStyle.Right,
                Width = 250,
                BackColor = Color.FromArgb(24, 24, 37),
                Padding = new Padding(10)
            };

            picDetail = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(230, 150),
                BackColor = Color.FromArgb(24, 24, 37),
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.None
            };

            lblDetailTitle = new Label { Location = new Point(10, 170), Size = new Size(230, 30), ForeColor = Color.FromArgb(203, 166, 247), Font = new Font("Segoe UI", 11f, FontStyle.Bold), AutoEllipsis = true };
            lblDetailType = new Label { Location = new Point(10, 205), Size = new Size(230, 22), ForeColor = Color.White, AutoEllipsis = true };
            lblDetailStatus = new Label { Location = new Point(10, 230), Size = new Size(230, 22), ForeColor = Color.White, AutoEllipsis = true };
            lblDetailRating = new Label { Location = new Point(10, 255), Size = new Size(230, 22), ForeColor = Color.FromArgb(249, 226, 175), Font = new Font("Segoe UI", 12f) };
            lblDetailGenre = new Label { Location = new Point(10, 280), Size = new Size(230, 22), ForeColor = Color.White, AutoEllipsis = true };
            lblDetailNotes = new Label { Location = new Point(10, 315), Size = new Size(230, 120), ForeColor = Color.FromArgb(180, 180, 200), AutoSize = false };
            lblDetailNotes.Font = new Font("Segoe UI", 9f);

            var sep = new Label { Location = new Point(10, 305), Size = new Size(230, 1), BackColor = Color.FromArgb(60, 60, 80), Text = "" };

            detailPanel.Controls.AddRange(new Control[] { picDetail, lblDetailTitle, lblDetailType, lblDetailStatus, lblDetailRating, lblDetailGenre, sep, lblDetailNotes });

            this.Controls.AddRange(new Control[] { mainPanel, detailPanel, sidePanel, topPanel });

            grid.SelectionChanged += Grid_SelectionChanged;

            // Events
            btnSearch.Click += (s, e) => LoadMovies();
            btnAdd.Click += BtnAdd_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnStats.Click += (s, e) => new StatsForm().ShowDialog();
            btnExport.Click += BtnExport_Click;
            searchBox.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadMovies(); };
            grid.CellDoubleClick += (s, e) => BtnEdit_Click(s, e);
            btnTheme.Click += BtnTheme_Click;
            grid.CellFormatting += (s, e) =>
            {
                if (e.ColumnIndex == grid.Columns["Rating"].Index && e.Value != null)
                {
                    int rating = Convert.ToInt32(e.Value);
                    e.Value = rating == 0 ? "-" : $"{rating} / 10";
                    e.FormattingApplied = true;
                }
            };
        }

        private ComboBox CreateCombo(string[] items, int y)
        {
            var cmb = new ComboBox
            {
                Location = new Point(10, y),
                Width = 190,
                BackColor = Color.FromArgb(40, 40, 60),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList,
                FlatStyle = FlatStyle.Flat
            };
            cmb.Items.AddRange(items);
            cmb.SelectedIndex = 0;
            return cmb;
        }

        private Button CreateButton(string text, int y, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(10, y),
                Width = 190,
                Height = 36,
                BackColor = color,
                ForeColor = Color.FromArgb(18, 18, 28),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderSize = 0 }
            };
        }

        private void LoadMovies()
        {
            string keyword = searchBox.Text.Trim();
            string type = cmbType.SelectedItem.ToString();
            string status = cmbStatus.SelectedItem.ToString();
            string genre = cmbGenre.SelectedItem.ToString();

            currentMovies = Database.Search(keyword, type, status, genre);

            grid.Rows.Clear();
            foreach (var m in currentMovies)
            {
                grid.Rows.Add(m.Id, m.Title, m.Type, m.Status, m.Rating, m.Genre, m.Notes, m.AddedDate);
            }

            lblCount.Text = $"共 {currentMovies.Count} 部影片";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            var form = new AddEditForm(null);
            if (form.ShowDialog() == DialogResult.OK)
                LoadMovies();
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("請先選擇一部影片！"); return; }
            int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            var movie = currentMovies.Find(m => m.Id == id);
            var form = new AddEditForm(movie);
            if (form.ShowDialog() == DialogResult.OK)
            {
                LoadMovies();
                // 編輯完後重新選取同一部影片
                foreach (DataGridViewRow row in grid.Rows)
                {
                    if ((int)row.Cells["Id"].Value == id)
                    {
                        row.Selected = true;
                        grid.CurrentCell = row.Cells["Title"];
                        break;
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) { MessageBox.Show("請先選擇一部影片！"); return; }
            if (MessageBox.Show("確定要刪除這部影片嗎？", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
                Database.Delete(id);
                LoadMovies();
                if (grid.Rows.Count > 0)
                {
                    grid.Rows[0].Selected = true;
                    grid.CurrentCell = grid.Rows[0].Cells["Title"];
                    Grid_SelectionChanged(null, null);
                }
                else
                {
                    ClearDetail();
                }
            }
        }

        private void CheckReminders()
        {
            string today = DateTime.Now.ToString("yyyy-MM-dd");
            var all = Database.GetAll();
            var reminders = all.FindAll(m => m.Reminder && m.WatchDate == today);

            if (reminders.Count > 0)
            {
                string msg = "📌 今日觀看提醒：\n\n";
                foreach (var m in reminders)
                    msg += $"🎬 {m.Title}（{m.Type} / {m.Genre}）\n";
                msg += "\n記得今天要看喔！";
                MessageBox.Show(msg, "觀看提醒", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void ClearDetail()
        {
            picDetail.Image = null;
            picDetail.BackColor = Color.FromArgb(40, 40, 60);
            lblDetailTitle.Text = "";
            lblDetailType.Text = "";
            lblDetailStatus.Text = "";
            lblDetailRating.Text = "";
            lblDetailGenre.Text = "";
            lblDetailNotes.Text = "";
        }
        private void BtnTheme_Click(object sender, EventArgs e)
        {
            isDarkMode = !isDarkMode;

            Color bg = isDarkMode ? Color.FromArgb(18, 18, 28) : Color.FromArgb(240, 240, 245);
            Color panelBg = isDarkMode ? Color.FromArgb(24, 24, 37) : Color.FromArgb(255, 255, 255);
            Color sideBg = isDarkMode ? Color.FromArgb(24, 24, 37) : Color.FromArgb(225, 225, 235);
            Color topBg = isDarkMode ? Color.FromArgb(30, 30, 46) : Color.FromArgb(210, 210, 230);
            Color textColor = isDarkMode ? Color.White : Color.FromArgb(30, 30, 46);
            Color gridBg = isDarkMode ? Color.FromArgb(24, 24, 37) : Color.White;
            Color inputBg = isDarkMode ? Color.FromArgb(40, 40, 60) : Color.White;
            Color labelColor = isDarkMode ? Color.FromArgb(180, 180, 200) : Color.FromArgb(60, 60, 80);

            this.BackColor = bg;
            topPanel.BackColor = topBg;
            sidePanel.BackColor = sideBg;
            mainPanel.BackColor = bg;

            lblTitle.ForeColor = isDarkMode ? Color.FromArgb(203, 166, 247) : Color.FromArgb(100, 80, 160);
            lblCount.ForeColor = isDarkMode ? Color.FromArgb(166, 227, 161) : Color.FromArgb(40, 140, 40);

            // 更新側邊欄所有控件
            foreach (Control c in sidePanel.Controls)
            {
                if (c is Label lbl)
                    lbl.ForeColor = labelColor;
                else if (c is TextBox tb)
                {
                    tb.BackColor = inputBg;
                    tb.ForeColor = textColor;
                }
                else if (c is ComboBox cmb)
                {
                    cmb.BackColor = inputBg;
                    cmb.ForeColor = textColor;
                }
            }

            // 更新 Grid
            grid.BackgroundColor = gridBg;
            grid.DefaultCellStyle.BackColor = gridBg;
            grid.DefaultCellStyle.ForeColor = textColor;
            grid.DefaultCellStyle.SelectionBackColor = isDarkMode ? Color.FromArgb(137, 180, 250) : Color.FromArgb(180, 200, 255);
            grid.DefaultCellStyle.SelectionForeColor = isDarkMode ? Color.Black : Color.Black;
            grid.ColumnHeadersDefaultCellStyle.BackColor = topBg;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = isDarkMode ? Color.FromArgb(203, 166, 247) : Color.FromArgb(100, 80, 160);
            grid.GridColor = isDarkMode ? Color.FromArgb(40, 40, 60) : Color.FromArgb(200, 200, 210);
            grid.AlternatingRowsDefaultCellStyle.BackColor = isDarkMode ? Color.FromArgb(30, 30, 46) : Color.FromArgb(245, 245, 250);
            grid.AlternatingRowsDefaultCellStyle.ForeColor = textColor;

            // 更新右側預覽區
            foreach (Control c in this.Controls)
            {
                if (c is Panel p && p.Dock == DockStyle.Right)
                {
                    p.BackColor = panelBg;
                    foreach (Control child in p.Controls)
                    {
                        if (child is Label l)
                            l.ForeColor = l.Font.Bold ? (isDarkMode ? Color.FromArgb(203, 166, 247) : Color.FromArgb(100, 80, 160)) : textColor;
                        else if (child is PictureBox pic)
                            pic.BackColor = inputBg;
                    }
                }
            }

            btnTheme.Text = isDarkMode ? "☀️  切換淺色主題" : "🌙  切換深色主題";
            grid.Refresh();
        }
        private void BtnExport_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                Filter = "CSV 檔案|*.csv",
                FileName = $"MovieTracker_{DateTime.Now:yyyyMMdd}.csv"
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                var sb = new System.Text.StringBuilder();
                sb.AppendLine("標題,類型,狀態,評分,風格,備註,新增日期,預計觀看日期,提醒");

                foreach (var m in Database.GetAll())
                {
                    sb.AppendLine($"\"{m.Title}\",{m.Type},{m.Status},{m.Rating},{m.Genre},\"{m.Notes}\",{m.AddedDate},{m.WatchDate},{(m.Reminder ? "是" : "否")}");
                }

                System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
                MessageBox.Show("✅ 匯出成功！", "匯出 CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void Grid_SelectionChanged(object sender, EventArgs e)
        {
            if (grid.SelectedRows.Count == 0) return;
            int id = (int)grid.SelectedRows[0].Cells["Id"].Value;
            var m = currentMovies.Find(x => x.Id == id);
            if (m == null) return;

            lblDetailTitle.Text = m.Title;
            lblDetailType.Text = "類型：" + m.Type;
            lblDetailStatus.Text = "狀態：" + m.Status;
            lblDetailRating.Text = m.Rating == 0 ? "評分：尚未觀看" : $"⭐ {m.Rating} / 10"; lblDetailGenre.Text = "風格：" + m.Genre;
            lblDetailNotes.Text = "備註：\n" + m.Notes;

            if (!string.IsNullOrEmpty(m.CoverUrl))
            {
                string fullPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images", m.CoverUrl);
                if (System.IO.File.Exists(fullPath))
                {
                    try { picDetail.Image = Image.FromFile(fullPath); }
                    catch { picDetail.Image = null; }
                }
                else
                    picDetail.Image = null;
            }
            else
            {
                picDetail.Image = null;
                picDetail.BackColor = Color.FromArgb(40, 40, 60);
            }
        }
    }
}