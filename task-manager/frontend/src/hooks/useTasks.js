import { useState, useCallback } from "react";
import axios from "axios";

const API_URL = `${import.meta.env.VITE_API_URL}/tasks`;

export const useTasks = () => {
  const [tasks, setTasks] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchTasks = useCallback(async () => {
    try {
      setLoading(true);
      const { data } = await axios.get(API_URL);
      setTasks(data);
    } catch (err) {
      setError("無法載入任務列表");
      console.error("載入失敗:", err);
    } finally {
      setLoading(false);
    }
  }, []);

  const addTask = useCallback(async (taskData) => {
    if (!taskData?.title?.trim()) return;
    setLoading(true);
    try {
      const { data } = await axios.post(API_URL, taskData);
      setTasks((prev) => [data, ...prev]);
      return data;
    } catch (err) {
      setError("新增任務失敗");
      console.error("新增失敗:", err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const updateTask = useCallback(async (task) => {
    setLoading(true);
    try {
      const { data } = await axios.put(`${API_URL}/${task.id}`, task);
      setTasks((prev) => prev.map((t) => (t.id === task.id ? data : t)));
      return data;
    } catch (err) {
      setError("更新任務失敗");
      console.error("更新失敗:", err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const deleteTask = useCallback(async (id) => {
    setLoading(true);
    try {
      await axios.delete(`${API_URL}/${id}`);
      setTasks((prev) => prev.filter((task) => task.id !== id));
    } catch (err) {
      setError("刪除任務失敗");
      console.error("刪除失敗:", err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  const searchTasks = useCallback(async (params = {}) => {
    try {
      setLoading(true);
      const { data } = await axios.get(API_URL, { params });
      setTasks(data);
      return data;
    } catch (err) {
      setError("查詢失敗");
      console.error("查詢失敗:", err);
      throw err;
    } finally {
      setLoading(false);
    }
  }, []);

  return {
    tasks,
    setTasks,
    loading,
    error,
    searchTasks,
    fetchTasks,
    addTask,
    updateTask,
    deleteTask,
  };
};
