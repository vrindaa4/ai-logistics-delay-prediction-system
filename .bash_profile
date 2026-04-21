
# >>> conda initialize >>>
# !! Contents within this block are managed by 'conda init' !!
__conda_setup="$('/Users/vrinda/anaconda3/bin/conda' 'shell.bash' 'hook' 2> /dev/null)"
if [ $? -eq 0 ]; then
    eval "$__conda_setup"
else
    if [ -f "/Users/vrinda/anaconda3/etc/profile.d/conda.sh" ]; then
        . "/Users/vrinda/anaconda3/etc/profile.d/conda.sh"
    else
        export PATH="/Users/vrinda/anaconda3/bin:$PATH"
    fi
fi
unset __conda_setup
# <<< conda initialize <<<


export PATH=${PATH}:/usr/local/mysql-9.0.1-macos14-arm64/bin

