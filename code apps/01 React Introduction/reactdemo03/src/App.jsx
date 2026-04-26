import { useState } from 'react'

function App() {
  const [title, setTitle] = useState('')
  const [todos, setTodos] = useState([])

  const addTodo = () => {
    const trimmedTitle = title.trim()

    if (!trimmedTitle) {
      return
    }

    const todo = {
      id: Date.now(),
      title: trimmedTitle,
      completed: false,
    }

    setTodos((prev) => [...prev, todo])
    setTitle('')
  }

  const deleteTodo = (id) => {
    setTodos((prev) => prev.filter((todo) => todo.id !== id))
  }

  const toggleTodo = (id) => {
    setTodos((prev) =>
      prev.map((todo) =>
        todo.id === id ? { ...todo, completed: !todo.completed } : todo
      )
    )
  }

  return (
    <main style={{ maxWidth: 560, margin: '40px auto', fontFamily: 'sans-serif' }}>
      <h1>React TodoList</h1>

      <section style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
        <input
          type="text"
          value={title}
          placeholder="请输入待办事项"
          onChange={(e) => setTitle(e.target.value)}
          onKeyDown={(e) => {
            if (e.key === 'Enter') {
              addTodo()
            }
          }}
          style={{ flex: 1, padding: '8px 10px' }}
        />
        <button type="button" onClick={addTodo}>
          添加
        </button>
      </section>

      <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'grid', gap: 8 }}>
        {todos.map((todo) => (
          <li
            key={todo.id}
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
              onChange={() => toggleTodo(todo.id)}
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
            <button type="button" onClick={() => deleteTodo(todo.id)}>
              删除
            </button>
          </li>
        ))}
      </ul>
    </main>
  )
}

export default App
