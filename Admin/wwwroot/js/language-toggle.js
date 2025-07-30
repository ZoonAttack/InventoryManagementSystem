const translations = {
    en: {
        appName: "Products Management System",
        loginTitle: "Admin Login",
        email: "Email",
        password: "Password",
        loginButton: "Login"
    },
    ar: {
        appName: "نظام إدارة المنتجات",
        loginTitle: "تسجيل دخول المدير",
        email: "البريد الإلكتروني",
        password: "كلمة المرور",
        loginButton: "تسجيل الدخول"
    }
};

function toggleDropdown() {
    const dropdown = document.getElementById("languageDropdown");
    dropdown.classList.toggle("hidden");
}

function setLanguage(lang) {
    localStorage.setItem('lang', lang);
    document.getElementById('lang-label').innerText = lang.toUpperCase();
    document.getElementById("languageDropdown").classList.add("hidden");
}

window.addEventListener("click", function (e) {
    const btn = document.querySelector(".dropdown-toggle");
    const dropdown = document.getElementById("languageDropdown");
    if (!btn.contains(e.target) && !dropdown.contains(e.target)) {
        dropdown.classList.add("hidden");
    }
});

document.addEventListener('DOMContentLoaded', () => {
    const savedLang = localStorage.getItem('lang') || 'en';
    document.getElementById('lang-label').innerText = savedLang.toUpperCase();
});