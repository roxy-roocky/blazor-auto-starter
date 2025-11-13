
window.ThemeSwitcher = {

    Light: 0,
    Dark: 1,
    setTheme: function(theme){
        console.log("[ThemeSwitcher] Set theme to", theme == this.Light ? "Light" : "Dark");
        document.documentElement.setAttribute("data-bs-theme", theme == this.Light ? "light" : "dark");
        document.cookie = "theme=" + theme;
    },

    getCurrentThemeFromCookie: function(){
        let cookie = document.cookie.split(";").find(row => row.startsWith("theme="));
        if(!cookie)
        {
            setTheme(this.Light);
            return this.Light;
        }
        let currentTheme = Number(cookie.split("=")[1]);
        console.log("[ThemeSwitcher] Fetch theme from cookie: ", currentTheme == this.Light ? "Light" : "Dark");

        return currentTheme;
    }
};