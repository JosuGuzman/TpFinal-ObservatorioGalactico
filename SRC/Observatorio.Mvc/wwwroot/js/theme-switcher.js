// Theme switcher functionality
document.addEventListener('DOMContentLoaded', function() {
    const themeToggle = document.getElementById('theme-toggle');
    const themeStyle = document.getElementById('theme-style');
    
    // Check for saved theme preference or prefer-color-scheme
    const prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)');
    const currentTheme = localStorage.getItem('theme') || 
                        (prefersDarkScheme.matches ? 'dark' : 'light');
    
    // Apply theme
    if (currentTheme === 'dark') {
        document.body.classList.add('dark-theme');
        if (themeToggle) themeToggle.checked = true;
    }
    
    // Theme toggle button functionality
    if (themeToggle) {
        themeToggle.addEventListener('change', function() {
            if (this.checked) {
                document.body.classList.add('dark-theme');
                localStorage.setItem('theme', 'dark');
            } else {
                document.body.classList.remove('dark-theme');
                localStorage.setItem('theme', 'light');
            }
        });
    }
    
    // Listen for system theme changes
    prefersDarkScheme.addEventListener('change', function(e) {
        if (!localStorage.getItem('theme')) {
            if (e.matches) {
                document.body.classList.add('dark-theme');
                if (themeToggle) themeToggle.checked = true;
            } else {
                document.body.classList.remove('dark-theme');
                if (themeToggle) themeToggle.checked = false;
            }
        }
    });
});