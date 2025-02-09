const express = require("express");
const cors = require("cors");
const bodyParser = require("body-parser");
const sqlite3 = require("sqlite3").verbose();
const path = require("path");
const errorHandler = require("./middleware/errorHandler");
const apiLimiter = require("./middleware/rateLimit");

const app = express();
const PORT = 5000;

// Middleware
app.use(cors());
app.use(bodyParser.json());
app.use(apiLimiter);

// 資料庫連接設置
const dbPath = path.join(__dirname, "db", "database.db");
const db = new sqlite3.Database(dbPath, (err) => {
  if (err) {
    console.error("❌ 無法連接 SQLite:", err.message);
    return;
  }
  console.log("✅ SQLite 資料庫已成功連接");

  // 初始化資料庫
  db.serialize(() => {
    // 檢查資料表是否存在，不存在才建立
    db.get(
      "SELECT name FROM sqlite_master WHERE type='table' AND name='tasks'",
      (err, row) => {
        if (err) {
          console.error("檢查資料表失敗:", err);
          return;
        }

        // 如果資料表不存在，才建立新的資料表
        if (!row) {
          db.run(
            `
          CREATE TABLE tasks (
            id INTEGER PRIMARY KEY AUTOINCREMENT,
            title TEXT NOT NULL,
            description TEXT,
            completed INTEGER DEFAULT 0,
            priority TEXT DEFAULT '低優先級',
            dueDate TEXT,
            createdAt TEXT DEFAULT CURRENT_TIMESTAMP
          )
        `,
            (err) => {
              if (err) {
                console.error("建立資料表失敗:", err);
              } else {
                console.log("✅ 資料表建立成功");
              }
            }
          );
        } else {
          console.log("✅ 資料表已存在，無需重建");
        }
      }
    );
  });
});

// API 路由
app.get("/", (req, res) => {
  res.send("🎉 Node.js Express API 伺服器運行中！");
});

app.get("/tasks", (req, res) => {
  const { status, priority } = req.query;

  let sql = "SELECT * FROM tasks WHERE 1=1";
  const params = [];

  // 根據完成狀態篩選
  if (status && status !== "全部") {
    sql += " AND completed = ?";
    params.push(status === "完成" ? 1 : 0);
  }

  // 根據優先級篩選
  if (priority && priority !== "全部") {
    sql += " AND priority = ?";
    params.push(priority);
  }

  // 加入排序
  sql += " ORDER BY id DESC";

  db.all(sql, params, (err, rows) => {
    if (err) {
      console.error("查詢任務失敗:", err);
      return res.status(500).json({ error: "查詢任務失敗" });
    }
    res.json(rows);
  });
});

app.post("/tasks", (req, res) => {
  const { title, description, priority, dueDate } = req.body;

  if (!title?.trim()) {
    return res.status(400).json({ error: "標題不能為空" });
  }

  const sql = `
    INSERT INTO tasks (title, description, priority, dueDate, completed)
    VALUES (?, ?, ?, ?, 0)
  `;

  db.run(sql, [title, description, priority, dueDate], function (err) {
    if (err) {
      console.error("新增任務失敗:", err);
      return res.status(500).json({ error: "新增任務失敗" });
    }

    // 回傳完整的任務資料
    const newTask = {
      id: this.lastID,
      title,
      description,
      priority,
      dueDate,
      completed: 0,
    };

    res.status(201).json(newTask);
  });
});

app.put("/tasks/:id", (req, res) => {
  const { id } = req.params;
  const { completed } = req.body;

  // 先獲取現有任務資料
  db.get("SELECT * FROM tasks WHERE id = ?", [id], (err, task) => {
    if (err) {
      console.error("獲取任務失敗:", err);
      return res.status(500).json({ error: "更新任務失敗" });
    }

    if (!task) {
      return res.status(404).json({ error: "找不到該任務" });
    }

    // 更新完成狀態
    const sql = `
      UPDATE tasks 
      SET completed = ?
      WHERE id = ?
    `;

    db.run(sql, [completed ? 1 : 0, id], function (err) {
      if (err) {
        console.error("更新任務失敗:", err);
        return res.status(500).json({ error: "更新任務失敗" });
      }

      // 回傳完整的任務資料
      res.json({
        ...task,
        completed: completed ? 1 : 0,
      });
    });
  });
});

app.delete("/tasks/:id", (req, res) => {
  const { id } = req.params;

  db.run("DELETE FROM tasks WHERE id = ?", id, function (err) {
    if (err) {
      console.error("刪除任務失敗:", err);
      return res.status(500).json({ error: "刪除任務失敗" });
    }
    res.json({ id: parseInt(id) });
  });
});

// 錯誤處理中間件
app.use(errorHandler);

// 啟動伺服器
app.listen(PORT, () => {
  console.log(`🚀 伺服器運行於 http://localhost:${PORT}`);
});
