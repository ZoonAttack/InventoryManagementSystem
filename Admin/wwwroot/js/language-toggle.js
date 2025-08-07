function setLanguage(lang) {
    localStorage.setItem('lang', lang);
    document.getElementById('lang-label').innerText = lang.toUpperCase();

    if (lang === 'ar') {
        document.getElementById('login-title').innerText = 'تسجيل دخول المشرف';
        document.getElementById('email-label').innerText = 'البريد الإلكتروني';
        document.getElementById('password-label').innerText = 'كلمة المرور';
        document.getElementById('login-button').innerText = 'تسجيل الدخول';
        document.documentElement.dir = 'rtl';  // اتجاه الصفحة يمين لليسار
        document.documentElement.lang = 'ar';
    } else {
        document.getElementById('login-title').innerText = 'Admin Login';
        document.getElementById('email-label').innerText = 'Email';
        document.getElementById('password-label').innerText = 'Password';
        document.getElementById('login-button').innerText = 'Login';
        document.documentElement.dir = 'ltr';  // اتجاه الصفحة يسار لليمين
        document.documentElement.lang = 'en';
    }
}

// التبديل بين إظهار/إخفاء القائمة المنسدلة عند الضغط على الزر
const langBtn = document.getElementById('lang-btn');
const dropdown = document.getElementById('lang-dropdown');

langBtn.addEventListener('click', () => {
    dropdown.classList.toggle('hide');
    langBtn.classList.toggle('active');
});

// إخفاء القائمة عند الضغط خارجها
document.addEventListener('click', (e) => {
    if (!e.target.closest('.language-toggle')) {
        dropdown.classList.add('hide');
        langBtn.classList.remove('active');
    }
});

// التعامل مع اختيار اللغة من القائمة
dropdown.querySelectorAll('a').forEach(link => {
    link.addEventListener('click', (e) => {
        e.preventDefault();
        const selectedLang = e.target.getAttribute('data-lang');
        setLanguage(selectedLang);
        dropdown.classList.add('hide');
        langBtn.classList.remove('active');
    });
});

// عند تحميل الصفحة – استرجاع اللغة وتطبيقها
document.addEventListener('DOMContentLoaded', () => {
    const lang = localStorage.getItem('lang') || 'en';
    setLanguage(lang);
});