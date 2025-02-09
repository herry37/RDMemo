const sqlite3 = require("sqlite3").verbose();
const { promisify } = require("util");

class TaskRepository {
  constructor(db) {
    this.db = db;
    // 將 callback 改為 Promise
    this.run = promisify(db.run.bind(db));
    this.all = promisify(db.all.bind(db));
  }

  async findAll() {
    return await this.all("SELECT * FROM tasks ORDER BY id DESC");
  }

  async create(title) {
    const result = await this.run(
      "INSERT INTO tasks (title, completed) VALUES (?, ?)",
      [title, 0]
    );
    return { id: result.lastID, title, completed: 0 };
  }
}

module.exports = TaskRepository;
