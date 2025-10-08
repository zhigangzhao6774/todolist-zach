import { ComponentFixture, TestBed } from '@angular/core/testing';
import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TodoListComponent } from './todo-list.component';
import { TodoService } from '../../services/todo.service';
import { TodoItem } from '../../models/todo-item.model';
import { of, throwError } from 'rxjs';

describe('TodoListComponent', () => {
    let component: TodoListComponent;
    let fixture: ComponentFixture<TodoListComponent>;
    let todoService: TodoService;
    let httpMock: HttpTestingController;

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

    beforeEach(async () => {
        await TestBed.configureTestingModule({
            imports: [TodoListComponent],
            providers: [
                TodoService,
                provideHttpClient(),
                provideHttpClientTesting()
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(TodoListComponent);
        component = fixture.componentInstance;
        todoService = TestBed.inject(TodoService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('should create', () => {
        expect(component).toBeTruthy();
    });

    describe('ngOnInit', () => {
        it('should load todos on init', () => {
            spyOn(component, 'loadTodos');
            component.ngOnInit();
            expect(component.loadTodos).toHaveBeenCalled();
        });
    });

    describe('loadTodos', () => {
        it('should load todos successfully', () => {
            spyOn(todoService, 'getTodos').and.returnValue(of(mockTodos));

            component.loadTodos();

            expect(component.isLoading()).toBe(false);
            expect(component.todos().length).toBe(2);
            expect(component.errorMessage()).toBe('');
        });

        it('should handle error when loading todos', () => {
            spyOn(todoService, 'getTodos').and.returnValue(
                throwError(() => new Error('API Error'))
            );

            component.loadTodos();

            expect(component.isLoading()).toBe(false);
            expect(component.errorMessage()).toBe('Failed to load TODO items. Make sure the API is running.');
        });
    });

    describe('addTodo', () => {
        it('should add a new todo', () => {
            const newTodo: TodoItem = {
                id: '3',
                title: 'New TODO',
                isCompleted: false,
                createdAt: new Date().toISOString()
            };

            component.newTodoTitle.set('New TODO');
            spyOn(todoService, 'createTodo').and.returnValue(of(newTodo));

            component.addTodo();

            expect(component.todos().length).toBe(1);
            expect(component.newTodoTitle()).toBe('');
        });

        it('should not add todo if title is empty', () => {
            component.newTodoTitle.set('');
            spyOn(todoService, 'createTodo');

            component.addTodo();

            expect(todoService.createTodo).not.toHaveBeenCalled();
        });

        it('should handle error when adding todo', () => {
            component.newTodoTitle.set('New TODO');
            spyOn(todoService, 'createTodo').and.returnValue(
                throwError(() => new Error('API Error'))
            );

            component.addTodo();

            expect(component.errorMessage()).toBe('Failed to create TODO item.');
        });
    });

    describe('toggleComplete', () => {
        it('should toggle todo completion status', () => {
            const todo = mockTodos[0];
            const updatedTodo = { ...todo, isCompleted: true };

            component.todos.set([todo]);
            spyOn(todoService, 'toggleTodoComplete').and.returnValue(of(updatedTodo));

            component.toggleComplete(todo);

            expect(component.todos()[0].isCompleted).toBe(true);
        });

        it('should handle error when toggling complete', () => {
            const todo = mockTodos[0];
            component.todos.set([todo]);
            spyOn(todoService, 'toggleTodoComplete').and.returnValue(
                throwError(() => new Error('API Error'))
            );

            component.toggleComplete(todo);

            expect(component.errorMessage()).toBe('Failed to update TODO item.');
        });
    });

    describe('deleteTodo', () => {
        it('should delete todo after confirmation', () => {
            const todo = mockTodos[0];
            component.todos.set([todo]);
            spyOn(window, 'confirm').and.returnValue(true);
            spyOn(todoService, 'deleteTodo').and.returnValue(of(void 0));

            component.deleteTodo(todo.id);

            expect(component.todos().length).toBe(0);
        });

        it('should not delete todo if not confirmed', () => {
            const todo = mockTodos[0];
            component.todos.set([todo]);
            spyOn(window, 'confirm').and.returnValue(false);
            spyOn(todoService, 'deleteTodo');

            component.deleteTodo(todo.id);

            expect(todoService.deleteTodo).not.toHaveBeenCalled();
            expect(component.todos().length).toBe(1);
        });
    });

    describe('edit operations', () => {
        it('should start editing', () => {
            const todo = mockTodos[0];
            component.startEdit(todo);

            expect(component.editingTodoId()).toBe(todo.id);
            expect(component.editingTitle()).toBe(todo.title);
        });

        it('should cancel editing', () => {
            component.editingTodoId.set('1');
            component.editingTitle.set('Test');

            component.cancelEdit();

            expect(component.editingTodoId()).toBeNull();
            expect(component.editingTitle()).toBe('');
        });

        it('should save edited todo', () => {
            const todo = mockTodos[0];
            const updatedTodo = { ...todo, title: 'Updated Title' };

            component.todos.set([todo]);
            component.editingTodoId.set(todo.id);
            component.editingTitle.set('Updated Title');

            spyOn(todoService, 'updateTodo').and.returnValue(of(updatedTodo));

            component.saveEdit(todo.id);

            expect(component.todos()[0].title).toBe('Updated Title');
            expect(component.editingTodoId()).toBeNull();
        });
    });

    describe('helper methods', () => {
        it('should check if todo is being edited', () => {
            component.editingTodoId.set('1');
            expect(component.isEditing('1')).toBe(true);
            expect(component.isEditing('2')).toBe(false);
        });

        it('should get completed count', () => {
            component.todos.set(mockTodos);
            expect(component.getCompletedCount()).toBe(1);
        });

        it('should get pending count', () => {
            component.todos.set(mockTodos);
            expect(component.getPendingCount()).toBe(1);
        });
    });
});
