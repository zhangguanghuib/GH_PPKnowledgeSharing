import { Component } from 'react'
import './App.css'
import ToDoItem from './ToDoItem'

class App extends Component {
  state = {
    nextId: 4,
    newTitle: '',
    todos: [
      { id: 1, title: '学习 React 类组件', completed: false },
      { id: 2, title: '实现 Todo 添加功能', completed: true },
      { id: 3, title: '实现 Todo 删除功能', completed: false },
    ],
  }

  handleTitleChange = (event) => {
    this.setState({ newTitle: event.target.value })
  }

  handleAddTodo = (event) => {
    event.preventDefault()

    const title = this.state.newTitle.trim()
    if (!title) {
      return
    }

    this.setState((prevState) => ({
      nextId: prevState.nextId + 1,
      newTitle: '',
      todos: [
        ...prevState.todos,
        { id: prevState.nextId, title, completed: false },
      ],
    }))
  }

  handleDeleteTodo = (id) => {
    this.setState((prevState) => ({
      todos: prevState.todos.filter((todo) => todo.id !== id),
    }))
  }

  handleToggleTodo = (id) => {
    this.setState((prevState) => ({
      todos: prevState.todos.map((todo) =>
        todo.id === id ? { ...todo, completed: !todo.completed } : todo,
      ),
    }))
  }

  render() {
    const { todos, newTitle } = this.state

    return (
      <main className="todo-page">
        <section className="todo-card">
          <h1>React TodoList (Class Component)</h1>

          <form className="todo-form" onSubmit={this.handleAddTodo}>
            <input
              type="text"
              value={newTitle}
              onChange={this.handleTitleChange}
              placeholder="请输入 Todo 标题"
            />
            <button type="submit">添加</button>
          </form>

          <ul className="todo-list">
            {todos.length === 0 ? (
              <li className="empty">暂无待办事项</li>
            ) : (
              todos.map((todo) => (
                <ToDoItem
                  key={todo.id}
                  todo={todo}
                  onToggle={this.handleToggleTodo}
                  onDelete={this.handleDeleteTodo}
                />
              ))
            )}
          </ul>
        </section>
      </main>
    )
  }
}

export default App
