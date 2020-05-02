#include <stdio.h>
#include <stdlib.h>

#include "data.h"
#include "extract/extract.h"
#include "my_function/students_pc.h"
#include "output/output.h"
#include "re_builder/re_builder.h"
#include "writer/writer.h"
void usage(void)
{
    printf("%s", "Usage: programm <flag>");
    printf("%s", "Available flags:");
    printf("%s", "-f to output data file");
    printf("%s", "-p to output special PCs");
    return;
}
int main(int argc, char *argv[])
{
    if (argc < 2)
	usage();
    else if(argc == 2)
    {
        info_pc *root_tmp = NULL;
        root_tmp = extract(root_tmp);
        printf("%s\n", "Welcome!");
        if (argv[1][0] == '-' && argv[1][1] == 'f')
        {
            output_pc(root_tmp);
            while(root_tmp->last){
                root_tmp = root_tmp->last;
            }
            getchar();
            printf("%s", "Goodbye!");
            return 0;
        }
        else if (argv[1][0] == '-' && argv[1][1] == 'p')
        {
	    info_pc *new_tmp;
            new_tmp = student_pc(root_tmp);
            getchar();
            printf("%s", "Goodbye!");
            return 0;
        }
        else
            usage();
    }
    else
        usage();
    return 0;
}
