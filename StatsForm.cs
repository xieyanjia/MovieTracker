using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using CartesianChart = LiveCharts.WinForms.CartesianChart;
using PieChart = LiveCharts.WinForms.PieChart;
using System.Media;

namespace MovieTracker
{
    public partial class StatsForm : Form
    {
        public StatsForm()
        {
            BuildUI();
            this.FormClosed += (s, e) =>
            {
                if (currentPlayer != null)
                {
                    currentPlayer.Stop();
                    currentPlayer.Close();
                    currentPlayer = null;
                }
            };
        }
        private static List<string> currentPlaylist = new List<string>();
        private static int currentTrackIndex = 0;

        private static System.Windows.Media.MediaPlayer currentPlayer;
        private void BuildUI()
        {
            this.Text = "📊 統計圖表";
            this.Size = new Size(900, 620);
            this.BackColor = Color.FromArgb(24, 24, 37);
            this.ForeColor = Color.White;
            this.Font = new Font("Segoe UI", 10f);
            this.StartPosition = FormStartPosition.CenterParent;

            var movies = Database.GetAll();

            var tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 24, 37),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10f)
            };

            tabControl.TabPages.Add(CreateStatusTab(movies));
            tabControl.TabPages.Add(CreateGenreTab(movies));
            tabControl.TabPages.Add(CreateRatingTab(movies));
            tabControl.TabPages.Add(CreateSummaryTab(movies));
            tabControl.TabPages.Add(CreateTop10Tab(movies));
            tabControl.SelectedIndexChanged += (s, e) =>
            {
                if (tabControl.SelectedTab?.Text == "🏆 Top 10 排行榜" && movies.Count > 0)
                {
                    var topRating = movies.Where(m => m.Rating > 0).Max(m => m.Rating);
                    var topMovies = movies.Where(m => m.Rating == topRating).ToList();
                    var topGenres = topMovies.GroupBy(m => m.Genre)
                                             .OrderByDescending(g => g.Count())
                                             .ToList();
                    int maxCount = topGenres[0].Count();
                    var tiedGenres = topGenres.Where(g => g.Count() == maxCount)
                                              .Select(g => g.Key)
                                              .ToList();
                    PlayGenreMusics(tiedGenres);
                }
                else
                {
                    if (currentPlayer != null)
                    {
                        currentPlayer.Stop();
                        currentPlayer.Close();
                        currentPlayer = null;
                    }
                }
            };

            this.Controls.Add(tabControl);
        }

        private TabPage CreateStatusTab(List<Movie> movies)
        {
            var page = new TabPage("📋 觀看狀態") { BackColor = Color.FromArgb(24, 24, 37) };

            if (movies.Count == 0)
            {
                page.Controls.Add(new Label
                {
                    Text = "還沒有任何影片！",
                    ForeColor = Color.FromArgb(180, 180, 200),
                    Font = new Font("Segoe UI", 12f),
                    Location = new Point(20, 20),
                    AutoSize = true
                });
                return page;
            }

            var statuses = new[] { "想看", "看過", "看一半" };
            var chart = new PieChart { Dock = DockStyle.Fill };
            var series = new SeriesCollection();

            foreach (var s in statuses)
            {
                int count = movies.Count(m => m.Status == s);
                if (count > 0)
                    series.Add(new PieSeries { Title = s, Values = new ChartValues<double> { count }, DataLabels = true, LabelPoint = p => $"{s}\n{p.Y}部 ({p.Participation:P0})" });
            }

            chart.Series = series;
            chart.LegendLocation = LegendLocation.Bottom;
            page.Controls.Add(chart);
            return page;
        }

        private TabPage CreateGenreTab(List<Movie> movies)
        {
            var page = new TabPage("🎭 風格分佈") { BackColor = Color.FromArgb(24, 24, 37) };

            if (movies.Count == 0)
            {
                page.Controls.Add(new Label
                {
                    Text = "還沒有任何影片！",
                    ForeColor = Color.FromArgb(180, 180, 200),
                    Font = new Font("Segoe UI", 12f),
                    Location = new Point(20, 20),
                    AutoSize = true
                });
                return page;
            }

            var genres = movies.GroupBy(m => m.Genre).OrderByDescending(g => g.Count()).ToList();
            var chart = new CartesianChart { Dock = DockStyle.Fill };
            var values = new ChartValues<double>();
            var labels = new List<string>();

            foreach (var g in genres)
            {
                values.Add(g.Count());
                labels.Add(g.Key);
            }

            chart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "影片數量",
                    Values = values,
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(137, 180, 250)),
                    DataLabels = true
                }
            };

            chart.AxisX.Add(new Axis { Labels = labels, Foreground = System.Windows.Media.Brushes.White });
            chart.AxisY.Add(new Axis { Title = "數量", Foreground = System.Windows.Media.Brushes.White, MinValue = 0, Separator = new LiveCharts.Wpf.Separator { Step = 1 } }); chart.BackColor = Color.FromArgb(24, 24, 37);
            page.Controls.Add(chart);
            return page;
        }

        private TabPage CreateRatingTab(List<Movie> movies)
        {
            var page = new TabPage("⭐ 評分分佈") { BackColor = Color.FromArgb(24, 24, 37) };

            if (movies.Count == 0)
            {
                page.Controls.Add(new Label
                {
                    Text = "還沒有任何影片！",
                    ForeColor = Color.FromArgb(180, 180, 200),
                    Font = new Font("Segoe UI", 12f),
                    Location = new Point(20, 20),
                    AutoSize = true
                });
                return page;
            }

            var chart = new CartesianChart { Dock = DockStyle.Fill };
            var values = new ChartValues<double>();
            var labels = new List<string>();

            for (int i = 1; i <= 10; i++)
            {
                int count = movies.Count(m => m.Rating == i);
                values.Add(count);
                labels.Add($"{i}分");
            }

            chart.Series = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "影片數量",
                    Values = values,
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(203, 166, 247)),
                    DataLabels = true
                }
            };

            chart.AxisX.Add(new Axis { Labels = labels, Foreground = System.Windows.Media.Brushes.White });
            chart.AxisY.Add(new Axis { Title = "數量", Foreground = System.Windows.Media.Brushes.White, MinValue = 0, Separator = new LiveCharts.Wpf.Separator { Step = 1 } }); chart.BackColor = Color.FromArgb(24, 24, 37);

            page.Controls.Add(chart);
            return page;
        }

        private TabPage CreateSummaryTab(List<Movie> movies)
        {
            var page = new TabPage("📈 總覽") { BackColor = Color.FromArgb(24, 24, 37) };

            int total = movies.Count;
            int watched = movies.Count(m => m.Status == "看過");
            int watching = movies.Count(m => m.Status == "看一半");
            int wantWatch = movies.Count(m => m.Status == "想看");
            double avgRating = total > 0 ? movies.Where(m => m.Rating > 0).Average(m => m.Rating) : 0;
            int movies_count = movies.Count(m => m.Type == "電影");
            int series_count = movies.Count(m => m.Type == "影集");

            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = true, Padding = new Padding(20) };

            AddStatCard(panel, "🎬 總影片數", total.ToString(), Color.FromArgb(137, 180, 250));
            AddStatCard(panel, "✅ 已看過", watched.ToString(), Color.FromArgb(166, 227, 161));
            AddStatCard(panel, "⏸️ 看一半", watching.ToString(), Color.FromArgb(250, 179, 135));
            AddStatCard(panel, "📌 想看", wantWatch.ToString(), Color.FromArgb(203, 166, 247));
            AddStatCard(panel, "⭐ 平均評分", avgRating.ToString("F1"), Color.FromArgb(249, 226, 175));
            AddStatCard(panel, "🎥 電影", movies_count.ToString(), Color.FromArgb(137, 220, 235));
            AddStatCard(panel, "📺 影集", series_count.ToString(), Color.FromArgb(243, 139, 168));

            page.Controls.Add(panel);
            return page;
        }

        private void AddStatCard(FlowLayoutPanel panel, string title, string value, Color color)
        {
            var card = new Panel { Size = new Size(180, 120), Margin = new Padding(10), BackColor = Color.FromArgb(30, 30, 46) };
            var lblTitle = new Label { Text = title, ForeColor = Color.FromArgb(180, 180, 200), Font = new Font("Segoe UI", 9f), Location = new Point(10, 15), AutoSize = true };
            var lblValue = new Label { Text = value, ForeColor = color, Font = new Font("Segoe UI", 28f, FontStyle.Bold), Location = new Point(10, 40), AutoSize = true };
            card.Controls.AddRange(new Control[] { lblTitle, lblValue });
            panel.Controls.Add(card);
        }
        private void PlayGenreMusic(string genre)
        {
            PlayGenreMusics(new List<string> { genre });
        }

        private void PlayGenreMusics(List<string> genres)
        {
            var genreFiles = new Dictionary<string, string>
    {
        { "動作", "action.mp3" },
        { "喜劇", "comedy.mp3" },
        { "愛情", "romance.mp3" },
        { "恐怖", "horror.mp3" },
        { "科幻", "scifi.mp3" },
        { "劇情", "drama.mp3" },
        { "動畫", "anime.mp3" },
        { "其他", "other.mp3" }
    };

            currentPlaylist = genres
                .Where(g => genreFiles.ContainsKey(g))
                .Select(g => System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, genreFiles[g]))
                .Where(p => System.IO.File.Exists(p))
                .ToList();

            if (currentPlaylist.Count == 0) return;

            currentTrackIndex = 0;

            if (currentPlayer != null)
            {
                currentPlayer.Stop();
                currentPlayer.Close();
            }

            currentPlayer = new System.Windows.Media.MediaPlayer();
            currentPlayer.MediaEnded += (s, e) =>
            {
                currentTrackIndex = (currentTrackIndex + 1) % currentPlaylist.Count;
                currentPlayer.Open(new Uri(currentPlaylist[currentTrackIndex]));
                currentPlayer.Play();
            };
            currentPlayer.Open(new Uri(currentPlaylist[0]));
            currentPlayer.Play();
        }
        private TabPage CreateTop10Tab(List<Movie> movies)
        {
            var page = new TabPage("🏆 Top 10 排行榜") { BackColor = Color.FromArgb(24, 24, 37) };

            var top10 = movies.Where(m => m.Rating > 0)
                             .OrderByDescending(m => m.Rating)
                             .Take(10)
                             .ToList();

            int topScore = top10.Count > 0 ? top10[0].Rating : 0;

            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true };

            int y = 20;
            int rank = 1;

            foreach (var m in top10)
            {
                int currentScore = m.Rating;
                int actualRank = top10.Count(x => x.Rating > currentScore) + 1;
                string rankText = actualRank == 1 ? "🥇" :
                                  actualRank == 2 ? "🥈" :
                                  actualRank == 3 ? "🥉" : $"#{actualRank}";
                Color rankColor = actualRank == 1 ? Color.FromArgb(249, 226, 175) :
                                  actualRank == 2 ? Color.FromArgb(180, 180, 180) :
                                  actualRank == 3 ? Color.FromArgb(210, 140, 100) :
                                  Color.FromArgb(137, 180, 250);
                var card = new Panel
                {
                    Location = new Point(20, y),
                    Size = new Size(820, 60),
                    BackColor = Color.FromArgb(30, 30, 46)
                };


                var lblRank = new Label
                {
                    Text = rankText,
                    Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                    ForeColor = rankColor,
                    Location = new Point(10, 12),
                    AutoSize = true
                };

                var lblTitle = new Label
                {
                    Text = m.Title,
                    Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                    ForeColor = Color.White,
                    Location = new Point(60, 8),
                    AutoSize = true
                };

                var lblInfo = new Label
                {
                    Text = $"{m.Type} · {m.Genre} · {m.Status}",
                    Font = new Font("Segoe UI", 9f),
                    ForeColor = Color.FromArgb(180, 180, 200),
                    Location = new Point(60, 32),
                    AutoSize = true
                };

                var lblStars = new Label
                {
                    Text = $"⭐ {m.Rating}/10",
                    Font = new Font("Segoe UI", 14f),
                    ForeColor = Color.FromArgb(249, 226, 175),
                    Location = new Point(680, 16),
                    AutoSize = true
                };

                card.Controls.AddRange(new Control[] { lblRank, lblTitle, lblInfo, lblStars });
                panel.Controls.Add(card);
                y += 70;
                rank++;
            }

            if (top10.Count > 0)
            {
                var tied = top10.Where(m => m.Rating == topScore).ToList();
                var tiedGenreGroups = tied.GroupBy(m => m.Genre).OrderByDescending(g => g.Count()).ToList();
                int maxGenreCount = tiedGenreGroups[0].Count();
                var tiedGenres = tiedGenreGroups.Where(g => g.Count() == maxGenreCount).Select(g => g.Key).ToList();

                string musicGenre = string.Join("、", tiedGenres);
                string tiedText;

                if (tied.Count > 1 && tiedGenres.Count > 1)
                {
                    string tiedTitles = string.Join("、", tied.Select(m => $"「{m.Title}」"));
                    tiedText = $"🎵 {tiedTitles} 並列第一（{topScore}分），{musicGenre}風格同樣並列，交替播放{musicGenre}風格背景音樂 🎶";
                }
                else if (tied.Count > 1)
                {
                    string tiedTitles = string.Join("、", tied.Select(m => $"「{m.Title}」"));
                    int actionCount = tied.Count(m => m.Genre == musicGenre);
                    tiedText = $"🎵 {tiedTitles} 並列第一（{topScore}分），其中{musicGenre}類有{actionCount}部最多，自動播放{musicGenre}風格背景音樂 🎶";
                }
                else
                {
                    tiedText = $"🎵 第一名「{top10[0].Title}」是{top10[0].Genre}類，自動播放{top10[0].Genre}風格背景音樂 🎶";
                }

                var lblPlaying = new Label
                {
                    Text = tiedText,
                    Location = new Point(20, y + 10),
                    Size = new Size(820, 60),
                    ForeColor = Color.FromArgb(203, 166, 247),
                    Font = new Font("Segoe UI", 10f, FontStyle.Italic),
                    AutoSize = false,
                    AutoEllipsis = false
                };
                panel.Controls.Add(lblPlaying);
            }

            if (top10.Count == 0)
            {
                panel.Controls.Add(new Label
                {
                    Text = "還沒有評分的影片！",
                    ForeColor = Color.FromArgb(180, 180, 200),
                    Font = new Font("Segoe UI", 12f),
                    Location = new Point(20, 20),
                    AutoSize = true
                });
            }

            page.Controls.Add(panel);
            return page;
        }
    }
}