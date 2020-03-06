#! /usr/bin/bash

mkdir MerkulovLab20
cd MerkulovLab20
echo "Hello, world">file1|echo "Hello, Dear">file2|cat file1 file2 >> file3
cmp -b file1 file2
comm file1 file2
wc file1 file2
diff -d --minimal file1 file2
sort -f file1 file2
tail -v file1 file2
echo "Hello"|tee -a file1 file2
uniq -c file1
od file1 file2
sum file1 file2
touch file3|echo "Hello, everyone!">file3
head file1 file2 file3
gzip -v file1|ls
gzip -d file1
bzip2 -v file2
bzip2 -d file2.bz2
paste -s file1 file2 file3
du -c file1 file2 file3
df -a file1
touch Warrior.ragdoll|touch Mage.class|touch Ninja.model|find -name '*.*l*'
find -name '*.*l*'|xargs -P 4 rm
touch file1 file2 file3 file4 file5 file6 file7|echo "Hello">file1|echo "Today">file2|echo "Is a">file3|echo "Nice">file4|echo "Day ">file5|echo "For ">file6|echo "Labs!">file7|cat file1 file2 file3 file4 file5 file6 file7 >> text|find -name "file*"|xargs -P 4 rm|split -b 5 text|find -name "xa*"|xargs -P 4 cat
find -name "*x*"|xargs -P 4 rm
cd ..
rmdir MerkulovLab20
echo " "
echo "Спасибо за внимание!"
