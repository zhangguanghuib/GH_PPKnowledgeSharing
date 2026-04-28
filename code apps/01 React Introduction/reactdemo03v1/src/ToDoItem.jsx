function ToDoItem({ todo, onToggle, onDelete }) {
  return (
    <li
      style={{
        display: 'flex',
        alignItems: 'center',
        gap: 8,
        border: '1px solid #ddd',
        padding: '8px 10px',
        borderRadius: 6,
      }}
    >
      <input
        type="checkbox"
        checked={todo.completed}
        onChange={() => onToggle(todo.id)}
      />
      <span
        style={{
          flex: 1,
          textDecoration: todo.completed ? 'line-through' : 'none',
          opacity: todo.completed ? 0.6 : 1,
        }}
      >
        {todo.title}
      </span>
      <button type="button" onClick={() => onDelete(todo.id)}>
        删除
      </button>
    </li>
  )
}

export default ToDoItem
