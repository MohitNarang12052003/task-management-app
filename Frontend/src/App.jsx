import { useState, useEffect, useCallback } from 'react';
import { taskApi } from './api';
import { useToast } from './useToast';
import './index.css';

const STATUS_OPTIONS = ['Pending', 'InProgress', 'Completed'];
const FILTER_OPTIONS = ['All', ...STATUS_OPTIONS];

const STATUS_ICONS = { Pending: '🕐', InProgress: '⚡', Completed: '✅' };

function formatDate(iso) {
  return new Date(iso).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' });
}

/* ── Toast Component ─────────────────────────────────────────── */
function ToastList({ toasts }) {
  return (
    <div className="toast-container">
      {toasts.map(t => (
        <div key={t.id} className={`toast ${t.type}`}>
          <span>{t.type === 'success' ? '✓' : '✕'}</span>
          {t.message}
        </div>
      ))}
    </div>
  );
}

/* ── Add Task Form ───────────────────────────────────────────── */
function AddTaskForm({ onAdd }) {
  const [form, setForm] = useState({ title: '', description: '', status: 'Pending' });
  const [loading, setLoading] = useState(false);

  function set(field, val) { setForm(f => ({ ...f, [field]: val })); }

  async function handleSubmit(e) {
    e.preventDefault();
    if (!form.title.trim()) return;
    setLoading(true);
    try {
      await onAdd(form);
      setForm({ title: '', description: '', status: 'Pending' });
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="card">
      <p className="card-title">✦ New Task</p>
      <form onSubmit={handleSubmit}>
        <div className="form-grid">
          <div className="form-group full-width">
            <label htmlFor="new-title">Title *</label>
            <input
              id="new-title"
              type="text"
              placeholder="What needs to be done?"
              value={form.title}
              onChange={e => set('title', e.target.value)}
              required
            />
          </div>
          <div className="form-group full-width">
            <label htmlFor="new-desc">Description</label>
            <textarea
              id="new-desc"
              rows={2}
              placeholder="Add details (optional)"
              value={form.description}
              onChange={e => set('description', e.target.value)}
            />
          </div>
          <div className="form-group">
            <label htmlFor="new-status">Status</label>
            <select id="new-status" value={form.status} onChange={e => set('status', e.target.value)}>
              {STATUS_OPTIONS.map(s => <option key={s} value={s}>{s}</option>)}
            </select>
          </div>
        </div>
        <div className="form-actions">
          <button type="submit" className="btn btn-primary" disabled={loading || !form.title.trim()}>
            {loading ? '⏳ Adding…' : '＋ Add Task'}
          </button>
        </div>
      </form>
    </div>
  );
}

/* ── Task Card ───────────────────────────────────────────────── */
function TaskCard({ task, onUpdate, onDelete }) {
  const [editing, setEditing]   = useState(false);
  const [saving,  setSaving]    = useState(false);
  const [deleting, setDeleting] = useState(false);
  const [form, setForm] = useState({
    title: task.title, description: task.description, status: task.status
  });

  function set(field, val) { setForm(f => ({ ...f, [field]: val })); }

  async function handleSave() {
    if (!form.title.trim()) return;
    setSaving(true);
    try { await onUpdate(task.id, form); setEditing(false); }
    finally { setSaving(false); }
  }

  async function handleDelete() {
    if (!window.confirm(`Delete "${task.title}"?`)) return;
    setDeleting(true);
    try { await onDelete(task.id); }
    finally { setDeleting(false); }
  }

  if (editing) {
    return (
      <div className="task-card editing">
        <div className="inline-edit">
          <input
            value={form.title}
            onChange={e => set('title', e.target.value)}
            placeholder="Title"
          />
          <textarea
            rows={2}
            value={form.description}
            onChange={e => set('description', e.target.value)}
            placeholder="Description"
          />
          <select value={form.status} onChange={e => set('status', e.target.value)}>
            {STATUS_OPTIONS.map(s => <option key={s} value={s}>{s}</option>)}
          </select>
          <div className="inline-edit-actions">
            <button className="btn btn-primary" onClick={handleSave} disabled={saving || !form.title.trim()}>
              {saving ? '⏳ Saving…' : '✓ Save'}
            </button>
            <button className="btn btn-ghost" onClick={() => setEditing(false)}>Cancel</button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="task-card">
      <div className="task-left">
        <div className="task-title">{task.title}</div>
        {task.description && <div className="task-desc">{task.description}</div>}
        <div className="task-meta">
          <span className={`status-badge status-${task.status}`}>
            {STATUS_ICONS[task.status]} {task.status}
          </span>
          <span className="task-date">🗓 {formatDate(task.createdAt)}</span>
        </div>
      </div>
      <div className="task-actions">
        <button className="btn btn-icon" title="Edit" onClick={() => setEditing(true)}>✏️</button>
        <button
          className="btn btn-danger-ghost"
          title="Delete"
          onClick={handleDelete}
          disabled={deleting}
        >
          {deleting ? '⏳' : '🗑'}
        </button>
      </div>
    </div>
  );
}

/* ── App ─────────────────────────────────────────────────────── */
export default function App() {
  const [tasks,   setTasks]   = useState([]);
  const [loading, setLoading] = useState(true);
  const [filter,  setFilter]  = useState('All');
  const { toasts, addToast }  = useToast();

  const fetchTasks = useCallback(async () => {
    try {
      const data = await taskApi.getAll();
      setTasks(data);
    } catch {
      addToast('Failed to load tasks. Is the API running?', 'error');
    } finally {
      setLoading(false);
    }
  }, [addToast]);

  useEffect(() => { fetchTasks(); }, [fetchTasks]);

  async function handleAdd(form) {
    try {
      const created = await taskApi.create(form);
      setTasks(prev => [created, ...prev]);
      addToast('Task created! 🎉');
    } catch (e) {
      addToast(e.message || 'Failed to create task', 'error');
      throw e;
    }
  }

  async function handleUpdate(id, form) {
    try {
      await taskApi.update(id, form);
      setTasks(prev => prev.map(t => t.id === id ? { ...t, ...form } : t));
      addToast('Task updated ✓');
    } catch (e) {
      addToast(e.message || 'Failed to update task', 'error');
      throw e;
    }
  }

  async function handleDelete(id) {
    try {
      await taskApi.delete(id);
      setTasks(prev => prev.filter(t => t.id !== id));
      addToast('Task deleted');
    } catch (e) {
      addToast(e.message || 'Failed to delete task', 'error');
      throw e;
    }
  }

  const filtered = filter === 'All' ? tasks : tasks.filter(t => t.status === filter);

  const counts = {
    All:        tasks.length,
    Pending:    tasks.filter(t => t.status === 'Pending').length,
    InProgress: tasks.filter(t => t.status === 'InProgress').length,
    Completed:  tasks.filter(t => t.status === 'Completed').length,
  };

  return (
    <div className="app-wrapper">
      {/* Header */}
      <header className="header">
        <div className="logo">
          <div className="logo-icon">⚡</div>
          <h1>Task<span>Flow</span></h1>
        </div>
        <div className="header-stats">
          <span className="stat-badge">📋 <strong>{counts.Pending}</strong> Pending</span>
          <span className="stat-badge">⚡ <strong>{counts.InProgress}</strong> In Progress</span>
          <span className="stat-badge">✅ <strong>{counts.Completed}</strong> Done</span>
        </div>
      </header>

      {/* Add Task */}
      <AddTaskForm onAdd={handleAdd} />

      {/* Filter Bar */}
      <div className="filter-bar">
        <span className="filter-label">Filter:</span>
        {FILTER_OPTIONS.map(f => (
          <button
            key={f}
            className={`filter-btn${filter === f ? ' active' : ''}`}
            onClick={() => setFilter(f)}
          >
            {f} ({counts[f] ?? 0})
          </button>
        ))}
      </div>

      {/* Task List */}
      {loading ? (
        <div className="loading-dots">
          <span /><span /><span />
        </div>
      ) : filtered.length === 0 ? (
        <div className="empty-state">
          <div className="empty-icon">📭</div>
          <h3>{filter === 'All' ? 'No tasks yet' : `No ${filter} tasks`}</h3>
          <p>{filter === 'All' ? 'Add your first task above to get started.' : 'Try a different filter.'}</p>
        </div>
      ) : (
        <div className="tasks-container">
          {filtered.map(task => (
            <TaskCard
              key={task.id}
              task={task}
              onUpdate={handleUpdate}
              onDelete={handleDelete}
            />
          ))}
        </div>
      )}

      <ToastList toasts={toasts} />
    </div>
  );
}
