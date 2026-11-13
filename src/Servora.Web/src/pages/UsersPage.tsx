import { useCallback, useEffect, useMemo, useState, type FormEvent } from 'react';
import * as usersApi from '@/api/usersApi';
import type { Role, UserResponse } from '@/api/types';
import { useAuth } from '@/auth/AuthContext';
import { getAssignableRoles, ROLE_LABELS } from '@/auth/roles';

type FormMode = 'create' | 'edit';

type UserFormState = {
  name: string;
  userName: string;
  password: string;
  role: Role;
  isActive: boolean;
};

const emptyForm = (defaultRole: Role): UserFormState => ({
  name: '',
  userName: '',
  password: '',
  role: defaultRole,
  isActive: true,
});

export function UsersPage() {
  const { getAccessToken, role, tenantId, branchId, userId } = useAuth();
  const assignableRoles = useMemo(() => getAssignableRoles(role), [role]);
  const defaultRole = assignableRoles.includes('Waiter')
    ? 'Waiter'
    : (assignableRoles[0] ?? 'Waiter');

  const [users, setUsers] = useState<UserResponse[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [formError, setFormError] = useState<string | null>(null);
  const [saving, setSaving] = useState(false);

  const [mode, setMode] = useState<FormMode | null>(null);
  const [editingUser, setEditingUser] = useState<UserResponse | null>(null);
  const [form, setForm] = useState<UserFormState>(() => emptyForm(defaultRole));

  const loadUsers = useCallback(async () => {
    setLoading(true);
    setError(null);

    try {
      const token = await getAccessToken();
      const list = await usersApi.listUsers(
        token,
        role === 'PlatformAdmin' ? tenantId : undefined,
      );
      setUsers(list);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Kullanıcılar yüklenemedi');
    } finally {
      setLoading(false);
    }
  }, [getAccessToken, role, tenantId]);

  useEffect(() => {
    void loadUsers();
  }, [loadUsers]);

  function openCreate() {
    setMode('create');
    setEditingUser(null);
    setFormError(null);
    setForm(emptyForm(defaultRole));
  }

  function openEdit(user: UserResponse) {
    setMode('edit');
    setEditingUser(user);
    setFormError(null);
    setForm({
      name: user.name,
      userName: user.userName,
      password: '',
      role: user.role,
      isActive: user.isActive,
    });
  }

  function closeForm() {
    setMode(null);
    setEditingUser(null);
    setFormError(null);
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSaving(true);
    setFormError(null);

    try {
      const token = await getAccessToken();

      if (mode === 'create') {
        if (role === 'PlatformAdmin' && form.role !== 'PlatformAdmin' && !tenantId) {
          throw new Error('Bu rol için tenant seçimi gerekli.');
        }

        await usersApi.createUser(token, {
          name: form.name.trim(),
          userName: form.userName.trim(),
          password: form.password,
          role: form.role,
          tenantId: role === 'PlatformAdmin' ? tenantId : undefined,
          branchId: role === 'Manager' ? branchId : undefined,
        });
      } else if (mode === 'edit' && editingUser) {
        await usersApi.updateUser(token, editingUser.id, {
          name: form.name.trim(),
          isActive: form.isActive,
          role: form.role,
          password: form.password.trim() ? form.password : null,
        });
      }

      closeForm();
      await loadUsers();
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Kayıt başarısız');
    } finally {
      setSaving(false);
    }
  }

  async function handleDelete(user: UserResponse) {
    if (user.id === userId) {
      setError('Kendi hesabını silemezsin.');
      return;
    }

    const confirmed = window.confirm(
      `"${user.name}" kullanıcısını silmek istediğine emin misin?`,
    );
    if (!confirmed) {
      return;
    }

    setError(null);
    try {
      const token = await getAccessToken();
      await usersApi.deleteUser(token, user.id);
      await loadUsers();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Silme başarısız');
    }
  }

  return (
    <div className="page-section">
      <div className="page-header">
        <div>
          <h1>Kullanıcılar</h1>
          <p className="muted">Şube kullanıcılarını yönet</p>
        </div>
        <button className="btn btn-primary" type="button" onClick={openCreate}>
          Yeni kullanıcı
        </button>
      </div>

      {error ? <p className="error">{error}</p> : null}

      {loading ? (
        <p className="muted">Yükleniyor...</p>
      ) : users.length === 0 ? (
        <p className="muted">Henüz kullanıcı yok.</p>
      ) : (
        <div className="table-wrap">
          <table className="data-table">
            <thead>
              <tr>
                <th>Ad</th>
                <th>Kullanıcı adı</th>
                <th>Rol</th>
                <th>Durum</th>
                <th />
              </tr>
            </thead>
            <tbody>
              {users.map((user) => (
                <tr key={user.id}>
                  <td>{user.name}</td>
                  <td>{user.userName}</td>
                  <td>{ROLE_LABELS[user.role]}</td>
                  <td>
                    <span
                      className={`status-pill${user.isActive ? ' is-active' : ''}`}
                    >
                      {user.isActive ? 'Aktif' : 'Pasif'}
                    </span>
                  </td>
                  <td className="table-actions">
                    <button
                      className="btn"
                      type="button"
                      onClick={() => openEdit(user)}
                    >
                      Düzenle
                    </button>
                    <button
                      className="btn"
                      type="button"
                      disabled={user.id === userId}
                      onClick={() => void handleDelete(user)}
                    >
                      Sil
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}

      {mode ? (
        <div className="modal-backdrop" role="presentation" onClick={closeForm}>
          <div
            className="card card-wide modal-card"
            role="dialog"
            aria-modal="true"
            aria-labelledby="user-form-title"
            onClick={(event) => event.stopPropagation()}
          >
            <h2 id="user-form-title">
              {mode === 'create' ? 'Yeni kullanıcı' : 'Kullanıcıyı düzenle'}
            </h2>

            <form className="form" onSubmit={handleSubmit}>
              <div className="field">
                <label htmlFor="user-name">Ad</label>
                <input
                  id="user-name"
                  value={form.name}
                  onChange={(e) => setForm((prev) => ({ ...prev, name: e.target.value }))}
                  required
                />
              </div>

              {mode === 'create' ? (
                <div className="field">
                  <label htmlFor="user-username">Kullanıcı adı</label>
                  <input
                    id="user-username"
                    value={form.userName}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, userName: e.target.value }))
                    }
                    required
                    autoComplete="off"
                  />
                </div>
              ) : (
                <div className="field">
                  <label>Kullanıcı adı</label>
                  <input value={form.userName} disabled />
                </div>
              )}

              <div className="field">
                <label htmlFor="user-role">Rol</label>
                <select
                  id="user-role"
                  value={form.role}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, role: e.target.value as Role }))
                  }
                >
                  {(assignableRoles.includes(form.role)
                    ? assignableRoles
                    : [form.role, ...assignableRoles]
                  ).map((item) => (
                    <option key={item} value={item}>
                      {ROLE_LABELS[item] ?? item}
                    </option>
                  ))}
                </select>
              </div>

              <div className="field">
                <label htmlFor="user-password">
                  {mode === 'create' ? 'Şifre' : 'Yeni şifre (opsiyonel)'}
                </label>
                <input
                  id="user-password"
                  type="password"
                  value={form.password}
                  onChange={(e) =>
                    setForm((prev) => ({ ...prev, password: e.target.value }))
                  }
                  required={mode === 'create'}
                  autoComplete="new-password"
                />
              </div>

              {mode === 'edit' ? (
                <label className="checkbox-field">
                  <input
                    type="checkbox"
                    checked={form.isActive}
                    onChange={(e) =>
                      setForm((prev) => ({ ...prev, isActive: e.target.checked }))
                    }
                  />
                  Aktif
                </label>
              ) : null}

              {formError ? <p className="error">{formError}</p> : null}

              <div className="actions">
                <button className="btn" type="button" onClick={closeForm}>
                  Vazgeç
                </button>
                <button className="btn btn-primary" type="submit" disabled={saving}>
                  {saving ? 'Kaydediliyor...' : 'Kaydet'}
                </button>
              </div>
            </form>
          </div>
        </div>
      ) : null}
    </div>
  );
}
