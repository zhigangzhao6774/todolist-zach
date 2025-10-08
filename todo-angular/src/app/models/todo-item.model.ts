export interface TodoItem {
    id: string;
    title: string;
    isCompleted: boolean;
    createdAt: string;
    updatedAt?: string;
}

export interface CreateTodoItemDto {
    title: string;
    isCompleted?: boolean;
}

export interface UpdateTodoItemDto {
    title?: string;
    isCompleted?: boolean;
}
