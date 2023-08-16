import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import { REFRESH_TOKEN_STORAGE, TOKEN_STORAGE } from '@utils/constants';
import { UserRole } from '@utils/enums';
import { api } from './service-api';
import { httpClient } from './service-axios';
import { IPasswordResetResponse, IUserProfile } from './types';

export interface ILogin {
  firstName: string;
  lastName: string;
  email: string;
  userId: string;
  token: string;
  refreshToken: string;
  role: UserRole;
  imageUrl: string;
}

export const useLogin = () => {
  return useMutation(
    [api.auth.login],
    ({ email, password }: { email: string; password: string }) => {
      return httpClient.post<ILogin>(api.auth.login, { email, password });
    },
    {
      onError: () => {},
      onSuccess: (data) => {
        localStorage.setItem(TOKEN_STORAGE, data?.data?.token);
        localStorage.setItem(REFRESH_TOKEN_STORAGE, data?.data?.refreshToken);
        localStorage.setItem('id', data?.data?.userId);
      },
    }
  );
};

export const useLogout = () => {
  return useMutation(
    [api.auth.logout],
    () => {
      return httpClient.post(api.auth.logout, {
        token: localStorage.getItem(REFRESH_TOKEN_STORAGE),
      });
    },
    {
      onSuccess: () => {
        localStorage.removeItem(TOKEN_STORAGE);
        localStorage.removeItem(REFRESH_TOKEN_STORAGE);
        localStorage.removeItem('id');
      },
    }
  );
};

export const useReAuth = () => {
  return useQuery(
    [api.auth.me],
    () => {
      return httpClient.get<IUserProfile>(api.auth.me);
    },
    {
      retry: false,
      refetchOnMount: false,
      refetchOnReconnect: false,
      refetchOnWindowFocus: false,
      retryOnMount: false,

      onError: () => {},
      select: (data) => data.data,
      enabled: !!localStorage.getItem(TOKEN_STORAGE),
      onSuccess: () => {},
    }
  );
};

export const useProfileAuth = (id: string) => {
  return useQuery(
    [api.auth.getUser(id)],
    () => {
      return httpClient.get<IUserProfile>(api.auth.getUser(id as string));
    },

    {
      onError: () => {},
      select: (data) => data.data,
      enabled: !!localStorage.getItem(TOKEN_STORAGE),
      onSuccess: () => {},
    }
  );
};

const changePassword = (data: {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}) => httpClient.post<IPasswordResetResponse>(api.auth.changePassword, data);

export const useChangePassword = () => {
  return useMutation([api.auth.changePassword], changePassword, {});
};

export const useForgotPassword = () => {
  return useMutation(
    [api.auth.forgotPassword],
    ({ email }: { email: string }) => {
      return httpClient.post<ILogin>(api.auth.forgotPassword, { email });
    },
    {}
  );
};

export interface ResponseData {
  data: string;
  message: string;
}
export interface ResponseDataToken {
  token: string;
  message: string;
}

export const useResetPasswordToken = () => {
  return useMutation(
    [api.auth.resetToken],
    (data: { email?: string | null; token: string }) => {
      return httpClient.post<ResponseDataToken>(api.auth.resetToken, data);
    }
  );
};

export const useResetPassword = () => {
  return useMutation(
    [api.auth.resetPassword],
    (data: {
      newPassword: string;
      confirmPassword: string;
      passwordChangeToken: string;
    }) => {
      return httpClient.post(api.auth.resetPassword, data);
    }
  );
};

export const useChangeEmail = () => {
  return useMutation(
    [api.auth.changeEmail],
    (data: {
      oldEmail: string;
      newEmail: string;
      confirmEmail: string;
      password: string;
    }) => {
      return httpClient.put<{ resendToken: string }>(
        api.auth.changeEmail,
        data
      );
    },
    {}
  );
};

export const useResendEmailVerification = () => {
  return useMutation(
    [api.auth.resendEmailVerification],
    (data: { token: string }) => {
      return httpClient.put(api.auth.resendEmailVerification, data);
    },
    {}
  );
};

export const useVerifyChangeEmail = (token: string) => {
  const url = `${api.auth.verifyChangeEmail}?token=${token}`;
  const queryClient = useQueryClient();

  return useQuery([api.auth.verifyChangeEmail], () => httpClient.get(url), {
    select: (data) => data.data,
    staleTime: Infinity,
    cacheTime: Infinity,
    retry: false,

    onSuccess: () => {
      queryClient.invalidateQueries([api.auth.me]);
    },
  });
};
