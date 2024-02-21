import { Text } from '@mantine/core';
import { useEffect } from 'react';
import { Outlet, useLocation, useNavigate } from 'react-router-dom';
import useAuth from '../../hooks/useAuth';

const NotRequiredAuth = () => {
  const auth = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const from = location.state?.from?.pathname || '/';
  const appVersion = localStorage.getItem('version');
  useEffect(() => {
    if (auth?.loggedIn) {
      if (from === '/login') {
        navigate('/404', { replace: true });
      } else {
        navigate(from, { replace: true });
      }
    }
  }, [auth?.loggedIn]);

  return (
    <>
      {!auth?.loggedIn && (
        <>
          <Outlet />
          <div style={{ position: 'fixed', top: '10px', right: '10px' }}>
            <Text c="dimmed" size="xs">
              v{appVersion}
            </Text>
          </div>
          <div style={{ position: 'fixed', bottom: '10px', right: '10px' }}>
            {/* <LanguageSelector /> */}
          </div>
        </>
      )}
    </>
  );
};

export default NotRequiredAuth;
