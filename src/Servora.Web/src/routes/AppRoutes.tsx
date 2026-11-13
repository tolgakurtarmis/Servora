import { Navigate, Route, Routes } from 'react-router-dom';
import { RequireRole } from '@/auth/RequireRole';
import { USER_MANAGER_ROLES } from '@/auth/roles';
import { AuthenticatedLayout } from '@/layout/AuthenticatedLayout';
import { LoginPage } from '@/pages/LoginPage';
import { PlaceholderPage } from '@/pages/PlaceholderPage';
import { UsersPage } from '@/pages/UsersPage';

export function AppRoutes() {
  return (
    <Routes>
      <Route path="/" element={<LoginPage />} />
      <Route path="/login" element={<Navigate to="/" replace />} />

      <Route element={<AuthenticatedLayout />}>
        <Route
          path="/masa-takip"
          element={
            <PlaceholderPage
              title="Masa Takip"
              description="Aktif masaları buradan takip edeceksin."
            />
          }
        />
        <Route
          path="/raporlar/gunluk"
          element={<PlaceholderPage title="Günlük Özet" />}
        />
        <Route
          path="/raporlar/satis"
          element={<PlaceholderPage title="Satış Raporu" />}
        />
        <Route
          path="/tanimlamalar/masalar"
          element={<PlaceholderPage title="Masalar" />}
        />
        <Route
          path="/tanimlamalar/menu"
          element={<PlaceholderPage title="Menü" />}
        />
        <Route
          path="/tanimlamalar/kullanicilar"
          element={
            <RequireRole roles={USER_MANAGER_ROLES}>
              <UsersPage />
            </RequireRole>
          }
        />
      </Route>

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
