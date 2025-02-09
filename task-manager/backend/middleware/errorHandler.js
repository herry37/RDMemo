const errorHandler = (err, req, res, next) => {
  console.error("錯誤:", err.message);

  if (err.type === "ValidationError") {
    return res.status(400).json({ error: err.message });
  }

  return res.status(500).json({
    error: "伺服器發生錯誤",
    detail: process.env.NODE_ENV === "development" ? err.message : undefined,
  });
};

module.exports = errorHandler;
