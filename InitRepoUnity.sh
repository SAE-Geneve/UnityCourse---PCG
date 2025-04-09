#!/bin/sh

git_url="https://github.com/SAE-Geneve/UnityCourse---PCG.git"
git_message="Init repo Unity"

echo "# UnityEmpty" >> README.md
git init

#Get an ignore file
curl -o ".gitignore" "https://raw.githubusercontent.com/github/gitignore/refs/heads/main/Unity.gitignore"
# Verify download
if [ $? -eq 0 ]; then
    echo "Download successful: .gitignore"
else
    echo "Failed to download .gitignore"

    read -p "Press Enter to exit."
    exit 1
fi


#Get an ignore file
curl -o ".gitattributes" "https://raw.githubusercontent.com/gitattributes/gitattributes/refs/heads/master/Unity.gitattributes"
# Verify download
if [ $? -eq 0 ]; then
    echo "Download successful: .gitignore"
else
    echo "Failed to download .gitignore"
    
    read -p "Press Enter to exit."
    exit 1
fi

git add .
git commit -m "$git_message"
git branch -M main
git remote add origin "$git_url"
git push -u origin main

# Pause the script
read -p "Press Enter to continue or Ctrl+C to exit."
echo "Resuming script..."