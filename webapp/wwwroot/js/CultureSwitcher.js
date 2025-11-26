window.CultureSwitcher = {

    setCulture: function(culture) {
        console.log("[CultureSwitcher] Set culture to", culture);
        document.cookie = "culture=" + culture + "; path=/; max-age=" + (365*24*60*60) + "; SameSite=Lax";
    },

    getCurrentCultureFromCookie: function() {
        let cookie = document.cookie.split(";").find(row => row.trim().startsWith("culture="));
        if (!cookie) {
            return null;
        }
        let currentCulture = cookie.split("=")[1].trim();
        console.log("[CultureSwitcher] Fetch culture from cookie:", currentCulture);
        return currentCulture;
    },

    getBrowserLanguage: function() {
        return navigator.language || navigator.userLanguage;
    }
};
