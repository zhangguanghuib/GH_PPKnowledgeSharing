import { Component } from 'react'

class ToDoItem extends Component {
  render() {
    const { todo, onToggle, onDelete } = this.props

    return (
      <li className="todo-item">
        <label>
          <input
            type="checkbox"
            checked={todo.completed}
            onChange={() => onToggle(todo.id)}
          />
          <span className="todo-id">#{todo.id}</span>
          <span className={todo.completed ? 'done' : ''}>{todo.title}</span>
        </label>

        <button
          type="button"
          className="delete-btn"
          onClick={() => onDelete(todo.id)}
        >
          删除
        </button>
      </li>
    )
  }
}

export default ToDoItem
