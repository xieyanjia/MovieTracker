# 🎬 Movie Tracker — 個人電影／追劇清單管理系統

---

## 一、專題簡介

Movie Tracker 是以 C# WinForms 開發的個人電影與追劇清單管理系統，搭配 SQLite 資料庫儲存資料。使用者可記錄想看、看過、看一半的電影與影集，並透過統計圖表了解自己的觀影習慣。

---

## 二、主要功能說明

| 功能 | 說明 |
|------|------|
| ➕ 新增影片 | 填寫標題、類型、狀態、風格、10分制評分、備註、封面圖等資訊後儲存。狀態為「想看」時隱藏評分；「看過」時隱藏預計觀看日期與提醒。 |
| ✏️ 編輯／🗑️ 刪除 | 點選列表中的影片後，點擊「編輯」修改資料，或點擊「刪除」並確認後移除。 |
| 🔍 搜尋篩選 | 左側可依標題關鍵字、類型、狀態、風格進行搜尋篩選。 |
| 🔔 今日提醒 | 程式啟動時自動檢查，若當天有設定提醒的影片，會彈出提醒視窗告知使用者。 |
| 📤 匯出 CSV | 點擊「匯出 CSV」選擇儲存位置，可將所有影片資料匯出為 Excel 可開啟的 CSV 檔案。 |
| 📊 統計圖表 | 包含觀看狀態圓餅圖、風格分佈、評分分佈長條圖、總覽統計卡片，以及 Top 10 排行榜。 |
| 🏆 Top 10 排行榜 | 依評分排名前10名，並列名次顯示相同獎牌。切換至此頁面時，會自動播放評分第一名的背景音樂，切換頁籤或關閉視窗時音樂自動停止。 |
| 🌙 主題切換 | 點擊「切換淺色／深色主題」按鈕，可在深色與淺色介面之間切換。 |

---

<img width="1264" height="690" alt="image" src="https://github.com/user-attachments/assets/7027b826-8557-4b24-b4b1-9c94f796f2d6" />
<img width="486" height="587" alt="image" src="https://github.com/user-attachments/assets/d9730a43-b3a8-4eba-9a08-f8269b83e18a" />
<img width="484" height="597" alt="image" src="https://github.com/user-attachments/assets/5ab6ff39-bc50-4f0e-87f1-72b39c537629" />
<img width="483" height="592" alt="image" src="https://github.com/user-attachments/assets/fecdedb3-eb49-4623-997b-44ed20b254c6" />
<img width="1261" height="798" alt="image" src="https://github.com/user-attachments/assets/c4f81afd-44e9-4b47-99e1-14bc32c05b7b" />
<img width="1263" height="696" alt="image" src="https://github.com/user-attachments/assets/a26b74d7-5b90-4f2e-9951-f1d3c92b96cd" />
<img width="1263" height="694" alt="image" src="https://github.com/user-attachments/assets/ea799ae1-0c98-4869-8699-576086182adf" />
<img width="1266" height="698" alt="image" src="https://github.com/user-attachments/assets/59d4c6b2-31f5-46ec-b337-55fe6095c511" />
<img width="1265" height="692" alt="image" src="https://github.com/user-attachments/assets/d7be7c84-0d8b-42a6-8370-b2421a54039f" />
<img width="1261" height="688" alt="image" src="https://github.com/user-attachments/assets/d5bc90d1-4225-4cfa-9c4e-04af2e11cc91" />
<img width="1261" height="687" alt="image" src="https://github.com/user-attachments/assets/b9c193d0-928a-463c-80e1-afac586c0fec" />
<img width="227" height="196" alt="image" src="https://github.com/user-attachments/assets/276da506-af12-418d-9c69-be83a0a81a7d" />

<img width="171" height="160" alt="image" src="https://github.com/user-attachments/assets/b333bc11-10d4-430e-8eba-e7f73b30aefd" />

<img width="881" height="607" alt="image" src="https://github.com/user-attachments/assets/42c16d28-1947-4812-b3a3-53a8fe6e5f95" />
<img width="878" height="609" alt="image" src="https://github.com/user-attachments/assets/586d01af-c147-427d-a913-e6ba583743c4" />
<img width="882" height="609" alt="image" src="https://github.com/user-attachments/assets/81cfcbf3-3636-4b1a-a22e-01241e7cebf4" />
<img width="878" height="607" alt="image" src="https://github.com/user-attachments/assets/8564a3da-908d-4657-b599-edcebccb0bc0" />
<img width="890" height="823" alt="image" src="https://github.com/user-attachments/assets/6baf91ed-c78f-427d-b1b4-de24581c7d93" />
<img width="1264" height="687" alt="image" src="https://github.com/user-attachments/assets/58bab71d-2ea9-4322-9d7e-2ba84db89deb" />


