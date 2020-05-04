#include <stdio.h>
#include <stdlib.h>
#include "data.h"
#include "my_function/student_pc.h"
#include "output/output.h"
#include "re_builder/re_builder.h"
#include "writer/writer.h"
void head(void)
{
    printf("%20s %9s %10s %10s %9s %10s", "Family", "CPU count", "CPU type","RAM Memory", "GPU type", "GPU memory");
    printf("%10s %10s %9s %11s %11s %20s\n", "HDD type", "HDD memory", "HDD count", "dcs control", "blt control", "OC");
}
int main(int argc, char *argv[])
{
    if (argc != 1)
    {
        info_pc *new, *tmp;
        int size = sizeof(info_pc);
        FILE *stream;
        stream = fopen(__NAME__, "r");
        if (!stream)
        {
            perror("Не удалось открыть файл");
            return 1;
        }
        new = (info_pc *)malloc(size);
        fread(new, size, 1, stream);
        new->last = NULL;

        while (!feof(stream) && new->next)
        {
            new->next = (info_pc *)malloc(size);
            fread(new->next, size, 1, stream);
            new->next->last = new;
            new = new->next;
        }
        fclose(stream);
        new->next = NULL;
        printf("Extracting...................OK\n");

        info_pc *root_tmp = NULL;
        root_tmp = new;
        printf("%s\n", "Welcome!");
        if (root_tmp)
        {
	    while (root_tmp->last)
                root_tmp = root_tmp->last;
            if (argv[1][0] == '-' && argv[1][1] == 'f')
            {
		head();
                output_pc(root_tmp);

                getchar();
                printf("%s", "Goodbye!");
                return 0;
            }
            else if (argv[1][0] == '-' && argv[1][1] == 'p')
            {
		head();
		student_pc(root_tmp);
                getchar();
                printf("%s", "Goodbye!");
                return 0;
            }
            else
            {
                printf("%s", "Errur at command line");
            }
        }
        else
        {
            printf("%s", "Database is empty menu");
        }
    }
    else
    {
        printf("%s", "Errur at command line");
    }
    return 0;
}
