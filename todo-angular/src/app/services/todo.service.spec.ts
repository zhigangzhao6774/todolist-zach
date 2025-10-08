import { TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TodoService } from './todo.service';
import { TodoItem, CreateTodoItemDto, UpdateTodoItemDto } from '../models/todo-item.model';

describe('TodoService', () => {
    let service: TodoService;
    let httpMock: HttpTestingController;
    const apiUrl = 'http://localhost:5162/api/todo';

    beforeEach(() => {
        TestBed.configureTestingModule({
            providers: [
                TodoService,
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        });
        service = TestBed.inject(TodoService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should be created', () => {
        expect(service).toBeTruthy();
    });

    describe('getTodos', () => {
        it('should return an Observable<TodoItem[]>', () => {
            const mockTodos: TodoItem[] = [
                {
                    id: '1',
                    title: 'Test TODO 1',
                    isCompleted: false,
                    createdAt: new Date().toISOString()
                },
                {
                    id: '2',
                    title: 'Test TODO 2',
                    isCompleted: true,
                    createdAt: new Date().toISOString()
                }
            ];

            service.getTodos().subscribe(todos => {
                expect(todos.length).toBe(2);
                expect(todos).toEqual(mockTodos);
            });

            const req = httpMock.expectOne(apiUrl);
            expect(req.request.method).toBe('GET');
            req.flush(mockTodos);
        });
    });

    describe('getTodoById', () => {
        it('should return a single todo', () => {
            const mockTodo: TodoItem = {
                id: '1',
                title: 'Test TODO',
                isCompleted: false,
                createdAt: new Date().toISOString()
            };

            service.getTodoById('1').subscribe(todo => {
                expect(todo).toEqual(mockTodo);
            });

            const req = httpMock.expectOne(`${apiUrl}/1`);
            expect(req.request.method).toBe('GET');
            req.flush(mockTodo);
        });
    });

    describe('createTodo', () => {
        it('should create a new todo', () => {
            const createDto: CreateTodoItemDto = {
                title: 'New TODO',
                isCompleted: false
            };

            const mockResponse: TodoItem = {
                id: '1',
                title: 'New TODO',
                isCompleted: false,
                createdAt: new Date().toISOString()
            };

            service.createTodo(createDto).subscribe(todo => {
                expect(todo).toEqual(mockResponse);
            });

            const req = httpMock.expectOne(apiUrl);
            expect(req.request.method).toBe('POST');
            expect(req.request.body).toEqual(createDto);
            req.flush(mockResponse);
        });
    });

    describe('updateTodo', () => {
        it('should update an existing todo', () => {
            const updateDto: UpdateTodoItemDto = {
                title: 'Updated TODO',
                isCompleted: true
            };

            const mockResponse: TodoItem = {
                id: '1',
                title: 'Updated TODO',
                isCompleted: true,
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            };

            service.updateTodo('1', updateDto).subscribe(todo => {
                expect(todo).toEqual(mockResponse);
            });

            const req = httpMock.expectOne(`${apiUrl}/1`);
            expect(req.request.method).toBe('PUT');
            expect(req.request.body).toEqual(updateDto);
            req.flush(mockResponse);
        });
    });

    describe('deleteTodo', () => {
        it('should delete a todo', () => {
            service.deleteTodo('1').subscribe(response => {
                expect(response).toBeUndefined();
            });

            const req = httpMock.expectOne(`${apiUrl}/1`);
            expect(req.request.method).toBe('DELETE');
            req.flush(null);
        });
    });

    describe('toggleTodoComplete', () => {
        it('should toggle the completion status', () => {
            const mockResponse: TodoItem = {
                id: '1',
                title: 'Test TODO',
                isCompleted: true,
                createdAt: new Date().toISOString(),
                updatedAt: new Date().toISOString()
            };

            service.toggleTodoComplete('1', true).subscribe(todo => {
                expect(todo.isCompleted).toBe(true);
            });

            const req = httpMock.expectOne(`${apiUrl}/1`);
            expect(req.request.method).toBe('PUT');
            expect(req.request.body).toEqual({ isCompleted: true });
            req.flush(mockResponse);
        });
    });
});
