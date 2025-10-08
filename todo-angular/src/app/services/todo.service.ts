import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TodoItem, CreateTodoItemDto, UpdateTodoItemDto } from '../models/todo-item.model';

@Injectable({
    providedIn: 'root'
})
export class TodoService {
    private readonly http = inject(HttpClient);
    private readonly apiUrl = 'http://localhost:5162/api/todo'; // Update port if different

    /**
     * Get all TODO items all the service must be injected in the constructor
     */
    getTodos(): Observable<TodoItem[]> {
        return this.http.get<TodoItem[]>(this.apiUrl);
    }

    /**
     * Get a specific TODO item by ID 
     */
    getTodoById(id: string): Observable<TodoItem> {
        return this.http.get<TodoItem>(`${this.apiUrl}/${id}`);
    }

    /**
     * Create a new TODO item
     */
    createTodo(dto: CreateTodoItemDto): Observable<TodoItem> {
        return this.http.post<TodoItem>(this.apiUrl, dto);
    }

    /**
     * Update an existing TODO item
     */
    updateTodo(id: string, dto: UpdateTodoItemDto): Observable<TodoItem> {
        return this.http.put<TodoItem>(`${this.apiUrl}/${id}`, dto);
    }

    /**
     * Delete a TODO item
     */
    deleteTodo(id: string): Observable<void> {
        return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }

    /**
     * Toggle the completed status of a TODO item
     */
    toggleTodoComplete(id: string, isCompleted: boolean): Observable<TodoItem> {
        return this.updateTodo(id, { isCompleted });
    }
}
