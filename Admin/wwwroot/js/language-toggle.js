// حفظ اللغة وتحديث الواجهة
function setLanguage(lang) {
    localStorage.setItem('lang', lang);
    document.getElementById('lang-label').innerText = lang.toUpperCase();

    if (lang === 'ar') {
        document.getElementById('login-title').innerText = 'تسجيل دخول المشرف';
        document.getElementById('email-label').innerText = 'البريد الإلكتروني';
        document.getElementById('password-label').innerText = 'كلمة المرور';
        document.getElementById('login-button').innerText = 'تسجيل الدخول';
    } else {
        document.getElementById('login-title').innerText = 'Admin Login';
        document.getElementById('email-label').innerText = 'Email';
        document.getElementById('password-label').innerText = 'Password';
        document.getElementById('login-button').innerText = 'Login';
    }
}

// عند تحميل الصفحة – استرجاع اللغة وتطبيقها
document.addEventListener('DOMContentLoaded', () => {
    const lang = localStorage.getItem('lang') || 'en';
    setLanguage(lang);
});
