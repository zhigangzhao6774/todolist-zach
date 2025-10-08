import { Component, signal, OnInit, computed } from '@angular/core';  // ✅ Add 'computed'
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TodoService } from '../../services/todo.service';
import { TodoItem } from '../../models/todo-item.model';

@Component({
    selector: 'app-todo-list',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './todo-list.component.html',
    styleUrl: './todo-list.component.css'
})
export class TodoListComponent implements OnInit {
    todos = signal<TodoItem[]>([]);
    newTodoTitle = signal('');
    isLoading = signal(false);
    errorMessage = signal('');
    editingId = signal<string | null>(null);
    editingTitle = '';

    // Sort functionality
    currentSortOption = signal<'date-newest' | 'date-oldest' | 'title-asc' | 'title-desc' | 'status'>('date-newest');

    // ✨ NEW: Search functionality
    searchText = signal('');

    // ✨ NEW: Filtered and sorted todos (computed automatically)
    filteredTodos = computed(() => {
        const search = this.searchText().toLowerCase().trim();
        const allTodos = this.todos();

        // If no search text, return all todos
        if (!search) {
            return allTodos;
        }

        // Filter todos by title
        return allTodos.filter(todo =>
            todo.title.toLowerCase().includes(search)
        );
    });

    constructor(private todoService: TodoService) { }

    ngOnInit(): void {
        this.loadTodos();
    }

    loadTodos(): void {
        this.isLoading.set(true);
        this.errorMessage.set('');

        this.todoService.getTodos().subscribe({
            next: (todos: TodoItem[]) => {
                this.todos.set(todos);
                this.sortTodos();
                this.isLoading.set(false);
            },
            error: (error: any) => {
                this.errorMessage.set('Failed to load TODO items. Please try again.');
                console.error('Error loading todos:', error);
                this.isLoading.set(false);
            }
        });
    }

    addTodo(): void {
        const title = this.newTodoTitle().trim();
        if (!title) return;

        this.isLoading.set(true);

        this.todoService.createTodo({ title, isCompleted: false }).subscribe({
            next: (newTodo: TodoItem) => {
                this.todos.update(current => [...current, newTodo]);
                this.sortTodos();
                this.newTodoTitle.set('');
                this.isLoading.set(false);
            },
            error: (error: any) => {
                this.errorMessage.set('Failed to add TODO item. Please try again.');
                console.error('Error adding todo:', error);
                this.isLoading.set(false);
            }
        });
    }

    toggleComplete(todo: TodoItem): void {
        const updatedTodo = {
            ...todo,
            isCompleted: !todo.isCompleted
        };

        this.todoService.updateTodo(todo.id, updatedTodo).subscribe({
            next: (updated: TodoItem) => {
                this.todos.update(current =>
                    current.map(t => t.id === todo.id ? updated : t)
                );
                this.sortTodos();
            },
            error: (error: any) => {
                this.errorMessage.set('Failed to update TODO item. Please try again.');
                console.error('Error updating todo:', error);
            }
        });
    }

    startEdit(todo: TodoItem): void {
        this.editingId.set(todo.id);
        this.editingTitle = todo.title;
    }

    saveEdit(id: string): void {
        const newTitle = this.editingTitle.trim();
        if (!newTitle) {
            this.cancelEdit();
            return;
        }

        const todo = this.todos().find(t => t.id === id);
        if (!todo) return;

        const updatedTodo = {
            ...todo,
            title: newTitle
        };

        this.todoService.updateTodo(id, updatedTodo).subscribe({
            next: (updated: TodoItem) => {
                this.todos.update(current =>
                    current.map(t => t.id === id ? updated : t)
                );
                this.sortTodos();
                this.cancelEdit();
            },
            error: (error: any) => {
                this.errorMessage.set('Failed to update TODO item. Please try again.');
                console.error('Error updating todo:', error);
            }
        });
    }

    cancelEdit(): void {
        this.editingId.set(null);
        this.editingTitle = '';
    }

    deleteTodo(id: string): void {
        if (!confirm('Are you sure you want to delete this TODO item?')) {
            return;
        }

        this.todoService.deleteTodo(id).subscribe({
            next: () => {
                this.todos.update(current => current.filter(t => t.id !== id));
            },
            error: (error: any) => {
                this.errorMessage.set('Failed to delete TODO item. Please try again.');
                console.error('Error deleting todo:', error);
            }
        });
    }

    isEditing(id: string): boolean {
        return this.editingId() === id;
    }

    getPendingCount(): number {
        return this.filteredTodos().filter(t => !t.isCompleted).length;  // ✅ Changed from todos() to filteredTodos()
    }

    getCompletedCount(): number {
        return this.filteredTodos().filter(t => t.isCompleted).length;  // ✅ Changed from todos() to filteredTodos()
    }

    // ✨ NEW: Clear search
    clearSearch(): void {
        this.searchText.set('');
    }

    // Sort todos based on selected option
    sortTodos(): void {
        const sortOption = this.currentSortOption();

        this.todos.update(current => {
            const sorted = [...current];

            switch (sortOption) {
                case 'date-newest':
                    return sorted.sort((a, b) =>
                        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
                    );

                case 'date-oldest':
                    return sorted.sort((a, b) =>
                        new Date(a.createdAt).getTime() - new Date(b.createdAt).getTime()
                    );

                case 'title-asc':
                    return sorted.sort((a, b) =>
                        a.title.localeCompare(b.title)
                    );

                case 'title-desc':
                    return sorted.sort((a, b) =>
                        b.title.localeCompare(a.title)
                    );

                case 'status':
                    return sorted.sort((a, b) => {
                        if (a.isCompleted === b.isCompleted) {
                            return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
                        }
                        return a.isCompleted ? 1 : -1;
                    });

                default:
                    return sorted;
            }
        });
    }

    // Change sort option
    changeSortOption(option: 'date-newest' | 'date-oldest' | 'title-asc' | 'title-desc' | 'status'): void {
        this.currentSortOption.set(option);
        this.sortTodos();
    }
}