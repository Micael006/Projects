#include <time.h>
#include <stdio.h>
#include <stdlib.h>

#include "data.h"
#include "./add_delete/add_delete.h"
#include "./output/output.h"
#include "./counting_cell/counting_cell.h"

int menu(void)
{
    printf("%s\n", "1. Add cell");
    printf("%s\n", "2. Remove cell");
    printf("%s\n", "3. Text output tree");
    printf("%s\n", "4. Count amount of cells with contained value power");//
    printf("%s\n", "5. Random filling");
    printf("%s\n", "6. Exit");
    int answer;
    scanf("%d", &answer);
    return answer;
}

int main()
{

    cell *root_tmp = NULL;
    printf("%s\n", "Welcome!");
    int k = 0;

    while (k != 6)
    {
        k = menu();
        switch (k)
        {
        case 1:
        {
            char value;
            getchar();
            printf("%s ", "Enter a char:");
            scanf("%c", &value);
            int val = value;
            if (!root_tmp)
            {
                root_tmp = create(val);
            }
            else
            {
                add(root_tmp, val);
            }
            printf("\n");
        }
        break;
        case 2:
        {
            char value;
            getchar();
            printf("%s ", "Enter a char:");
            scanf("%c", &value);
            int val = value;
            if (root_tmp)
            {
                if (!root_tmp->left && !root_tmp->right && val == root_tmp->value)
                {
                    root_tmp = destroy(root_tmp);
                }
                else
                {
                    if (!root_tmp->left && !root_tmp->right && val != root_tmp->value)
                    {
                        printf("%s\n", "This char doesn`t exist");
                    }
                    else
                    {
                        delete (root_tmp, root_tmp, val);
                    }
                }
                delete (root_tmp, root_tmp, val);
            }
            else
            {
                printf("%s\n", "Tree is empty, we haven't anything to remove yet");
            }
        }
        break;
        case 3:
        {
            getchar();
            text_out(root_tmp, 0);
        }
        break;
        case 4:
        {
	    int number = 0;
            if (root_tmp)
	    {
		number = count(root_tmp);
		printf("%s %d\n", "Number of cells with elment's value power is:", number);
	    }
	    else
	    {
		printf("%s\n","Tree is empty, so number of cells is obviosly 0"); 
	    }
        }
        break;
        case 5:
        {
            srand(time(NULL));
            if (!root_tmp)
            {
                root_tmp = create((rand() % 95 + 32));
            }
            for (int i = 0; i < 10; i++)
            {
                add(root_tmp, (rand() % 95 + 32));
            }
        }
        break;
        case 6:

            break;
        default:
            printf("%s\n", "Invalid argument, please type number from 1 to 6");
            break;
        }
    }

    printf("%s", "Goodbye, have a nice day!");
    return 0;
}
