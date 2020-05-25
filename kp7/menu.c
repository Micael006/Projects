#include <stdio.h>
#include <stdlib.h>
#include "vector/vector.h"

int k1, k2, k3 = 0;
void printf_full(cvector *matrix, cvector *ind, int k, int size_x, int size_y)
{
  float* elem;
  int index = -1;
  int count = k;
  int cur = 0;
  if(count > 0)
  {
    elem = (float*) cvector_get(matrix, cur);
    index = *(int*) cvector_get(ind, cur);
    cur++;
    count--;
  }
  for(int l = 0; l < size_x * size_y; l++)
  {
    if((!(l % size_y)) && (l))
      printf("\n");
    if(l == index)
      printf("%7.3f ", *elem);
    else
      printf("%7d ", 0);
    if((count > 0) && (l == index))
    {
       elem = (float*) cvector_get(matrix, cur);
       index = *(int*) cvector_get(ind, cur);
       count--;
       cur++;
    }
  }
  printf("\n");
}
void print_short(cvector *matrix_short, cvector *ind, int k)
{
  for (int i = 0; i < k; i++)
  {
    float *elem = (float*) cvector_get(matrix_short, i);
    if(elem != NULL)
      printf("%7.3f ", *elem);
  }
  printf("\n");
  for (int i = 0; i < k; i++)
  {
    int *elem = (int*) cvector_get(ind, i);
    if(elem != NULL)
      printf("%7d ", *elem);
  }
  printf("\n");
}
void multiply_short(cvector *matrix1, cvector *ind1, int k1, cvector *matrix2, cvector *ind2, int k2, cvector *matrix3, cvector *ind3, int n, int m, int p)
{
  int i1, j, j1, l1;
  float s;
  for (int i = 0; i < n; i++)
    for (int l = 0; l < p; l++)
    {
      s = 0;
      for (int t1 = 0; t1 < k1; t1++)
      {
        i1 = *(int*) cvector_get(ind1, t1) / m;
        j = *(int*) cvector_get(ind1, t1) % m;
        for (int t2 = 0; t2 < k2; t2++)
        {
          j1 = *(int*) cvector_get(ind2, t2) / p;
          l1 = *(int*) cvector_get(ind2, t2) % p;
          if (i == i1 && j == j1 && l == l1)
            s += (*(float*) cvector_get(matrix1, t1)) * (*(float*) cvector_get(matrix2, t2));
        }
      }
      if (s != 0)
      {
        cvector_push(matrix3, (void*)&s);
	int m = i * p + l;
	cvector_push(ind3, (void*)&m);
        k3++;
      }
    }
}

int main()
{ 
  cvector matrix1, matrix2, matrix3, ind1, ind2, ind3; 
  cvector_init(&matrix1, sizeof(float));
  cvector_init(&matrix2, sizeof(float));
  cvector_init(&matrix3, sizeof(float));
  cvector_init(&ind1, sizeof(int));
  cvector_init(&ind2, sizeof(int));
  cvector_init(&ind3, sizeof(int));
  int n, m, p, i, j;
  printf("Введите размеры n, m, p: ");
  scanf("%d %d %d", &n, &m, &p);
  printf("\n");
  printf("Введите количество ненулевых элементов в первой и во второй матрице: ");
  scanf("%d %d", &k1, &k2);
  printf("\n");
  for (i = 0; i < k1; i++)
  {
    float elem1;
    int elem2;
    scanf("%f %d", &elem1, &elem2);
    cvector_push(&matrix1, (void*)&elem1);
    cvector_push(&ind1, (void*)&elem2);
  }
  for (i = 0; i < k2; i++)
  {
    float elem1;
    int elem2;
    scanf("%f %d", &elem1, &elem2);
    cvector_push(&matrix2, (void*)&elem1);
    cvector_push(&ind2, (void*)&elem2);
  }
  cvector_sort(&matrix1, &ind1);
  cvector_sort(&matrix2, &ind2);
  multiply_short(&matrix1, &ind1, k1, &matrix2, &ind2, k2, &matrix3, &ind3, n, m, p);
  printf("Краткая форма матрицы 1: \n");
  print_short(&matrix1, &ind1, k1);
  printf("Краткая форма матрицы 2: \n");
  print_short(&matrix2, &ind2, k2);
  printf("Краткая форма результирующей матрицы: \n");
  print_short(&matrix3, &ind3, k3);
  printf("Полная форма матрицы 1: \n");
  printf_full(&matrix1, &ind1, k1, n, m);
  printf("Полная форма матрицы 2: \n");
  printf_full(&matrix2, &ind2, k2, m, p);
  printf("Полная форма результирующей матрицы: \n");
  printf_full(&matrix3, &ind3, k3, n, p);
  if((n != p) || (k3 > n)) 
  {
    printf("Результирующая матрица не является диагональной\n");
  }
  else 
  {
    int check = 0;
    for(int i = 0; i < k3; i++)
    {
      int index = *((int*) cvector_get(&ind3, i));
      if(((index / p) != (index % p)) && (p > 1))
  	check = 1;
    }	
    if(!check)
      printf("Результирующая матрица - диагональная\n");
    else
      printf("Результирующая матрица не является диагональной\n");
  }
  return 0;
}
