import { useState } from 'react'
import ToDoItem from './ToDoItem'

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
          <ToDoItem
            key={todo.id}
            todo={todo}
            onToggle={toggleTodo}
            onDelete={deleteTodo}
          />
        ))}
      </ul>
    </main>
  )
}

export default App
