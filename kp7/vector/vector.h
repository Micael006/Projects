#ifndef CVECTOR_H_
#define CVECTOR_H_
#include <stdio.h>
#include <stdlib.h>
#include <malloc.h>
#include <string.h> 
//Стандартный размер вектора при инициализации
#define CVECTOR_INIT_CAPACITY 4
 
typedef struct{
void** data;
int size;
int capacity;
size_t element_size;
} cvector;
 
/*
* Инициализация структуры вектора.
*
* __v указатель на структуру вектора
* __dataSize размер элемента данных в байтах
*/
void cvector_init(cvector* __v, size_t __dataSize);
 
//Функция, которая возвращает текущее количество элементов в векторе.
int cvector_size(cvector* __v);
//Функция добавления данных в конец вектора.
void cvector_push(cvector* __v, void* __data);
 
//Функция изменения элемента вектора по индексу.
int cvector_set(cvector* __v, int __index, void* __data);
 
// Удаление элемента из вектора по индексу.
int cvector_delete(cvector* __v, int __index);
 
//Получение значения элемента по индексу.
void* cvector_get(cvector* __v, int __index);
 
//Полная очистка вектора.
void cvector_clear(cvector* __v);
void cvector_resize(cvector* __v, int __newCap);
void cvector_sort(cvector* matrix, cvector* ind);
#endif
