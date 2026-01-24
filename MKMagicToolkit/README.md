# MKMagicToolkit

MKMagicToolKit에 포함되는 커스텀 디버그입니다.
로그는 빌드의 포함되지 않으며 디벨로먼트 모드를 키고 빌드 할 시 포함 됩니다.

# 📦 Patch Notes

---

## [1.1.2] - Log navigation fix

### 🐞 Fixed
- 콘솔에서 `CustomDebug` 로그를 더블 클릭하면  
  `CustomDebug` 클래스 파일로 이동하던 문제 수정
- 로그 호출 시 실제 호출 위치로 정확히 이동하도록 개선

### 🗑 Removed
- `CustomDebug`호출 부분을 무시하고 실제 로그 호출 부분으로 이동하는 `ConsoleCallerOpener` 제거

### 🔧 Changed
- `CustomDebug` 클래스에 `HideInCallstack` 속성 적용
- 로그 스택 트레이스에서 `CustomDebug` 호출 프레임 숨김

---

### Summary (EN)
- Fixed incorrect log navigation when clicking `CustomDebug` logs
- Removed unstable console hook implementation
- Improved stack trace clarity using `HideInCallstack`

---

## [1.1.1] - Namespace fix

### 🐞 Fixed
- `MonoSingleton` 클래스의 네임스페이스 선언 오류 수정

---

### Summary (EN)
- Fixed incorrect namespace declaration in `MonoSingleton`

---

## [1.1.0] - MonoSingleton added

### ✨ Added
- `MonoSingleton` 기능 추가
- Unity 환경에서 안전하게 동작하는 싱글톤 패턴 제공

---

### Summary (EN)
- Added `MonoSingleton` utility
- Introduced safe singleton pattern for Unity runtime

---

## [1.0.0] - Initial release

### 🎉 Added
- 기본 유틸리티 구조 초기 릴리스

---

### Summary (EN)
- Initial release of the utility package

