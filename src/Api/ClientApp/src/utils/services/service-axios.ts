import { REFRESH_TOKEN_STORAGE, TOKEN_STORAGE } from "@utils/constants";
import { BASE_URL } from "@utils/env";
import axios, {
  AxiosDefaults,
  AxiosError,
  AxiosRequestConfig,
  AxiosResponse,
} from "axios";
import { api } from "./service-api";

type RequestData = Record<string, any>;

const THREE_MINUTES = 3 * 60 * 1000;
const baseURL = BASE_URL;
const baseConfig = {
  baseURL,
  timeout: THREE_MINUTES,
  "Content-Type": "Application/json",
};
/**
 * Axios HTTP Client
//  * {@link https://github.com/axios/axios#request-config Axios Request Config}
 */

const axiosInstance = axios.create({
  baseURL: BASE_URL,
  timeout: 1000,
});

export const httpClient = {
  get: <T>(url: string, config?: AxiosRequestConfig<RequestData>) =>
    axiosInstance.get<T>(url, {
      ...baseConfig,
      ...config,
    }),

  post: <T>(
    url: string,
    data: RequestData,
    config?: AxiosRequestConfig<RequestData>
  ) =>
    axiosInstance.post<T>(url, data, {
      ...baseConfig,
      data,
      ...config,
    }),

  put: <T>(
    url: string,
    data: RequestData,
    config?: AxiosRequestConfig<RequestData>
  ) =>
    axiosInstance.put<T>(url, data, {
      ...baseConfig,
      ...config,
    }),

  patch: <T>(
    url: string,
    data?: RequestData,
    config?: AxiosRequestConfig<RequestData>
  ) =>
    axiosInstance.patch<T>(url, data, {
      ...baseConfig,
      ...config,
    }),
  delete: <T>(url: string, config?: AxiosRequestConfig<RequestData>) =>
    axiosInstance.delete<T>(url, {
      ...baseConfig,
      ...config,
    }),
};

axiosInstance.interceptors.request.use(
  async function (config: AxiosRequestConfig) {
    const token = localStorage.getItem("token");
    if (token && config.headers) {
      config.headers["Authorization"] = `Bearer ${token}`;
    }

    config.data = filterFalseyValues(config.data);
    if (
      config.headers &&
      config.headers["content-type"] === "multipart/form-data"
    ) {
      config.data = toFormData(config.data);
      delete config.headers["formData"];
    }
    return config;
  },
  function (error: any) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 404) {
      }
    }
    return Promise.reject(error);
  }
);

interface IFailedRequestQueue {
  onSuccess: (token: string) => void;
  onFailure: (error: AxiosError) => void;
}

let isRefreshing = false;
let failedRequestQueue: IFailedRequestQueue[] = [];

function handleRefreshToken(refreshToken: string) {
  isRefreshing = true;

  axios
    .post(`${baseURL}${api.auth.refreshToken}`, { token: refreshToken })
    .then((res) => {
      const { token } = res.data;

      localStorage.setItem(REFRESH_TOKEN_STORAGE, res.data?.refreshToken);
      localStorage.setItem(TOKEN_STORAGE, res.data?.token);
      // @ts-ignore

      failedRequestQueue.forEach((request) => request.onSuccess(token));
      failedRequestQueue = [];
    })
    .catch((error) => {
      failedRequestQueue.forEach((request) => request.onFailure(error));
      failedRequestQueue = [];

      localStorage.removeItem(REFRESH_TOKEN_STORAGE);
      localStorage.removeItem(TOKEN_STORAGE);
    })
    .finally(() => {
      isRefreshing = false;
    });
}

export function setAuthorizationHeader(
  request: AxiosDefaults | AxiosRequestConfig | any,
  token: string
) {
  request.headers.Authorization = `Bearer ${token}`;
}

axiosInstance.interceptors.response.use(
  function (response: AxiosResponse) {
    // Any status code that lie within the range of 2xx cause this function to trigger
    // Do something with response data
    return response;
  },
  function (error: any) {
    if (error?.response?.status === 401) {
      const originalConfig = error.config;
      const refreshToken = localStorage.getItem(
        REFRESH_TOKEN_STORAGE
      ) as string;
      !isRefreshing && handleRefreshToken(refreshToken);

      return new Promise((resolve, reject) => {
        failedRequestQueue.push({
          onSuccess: (token: string) => {
            setAuthorizationHeader(originalConfig, token);
            resolve(axiosInstance(originalConfig));
          },
          onFailure: (error: AxiosError) => {
            reject(error);
          },
        });
      });
    }
    return Promise.reject(error);
  }
);
/**
 * Remove empty, null and undefined values
 * @param obj a record of key value pair
 * @returns a record that does not have empty, null or undefined values
 */
export function filterFalseyValues(obj: Record<string, any>) {
  for (const propName in obj) {
    if (["", null, undefined].includes(obj[propName])) {
      delete obj[propName];
    } else if (
      obj[propName] instanceof Object &&
      Object.keys(obj[propName]).length
    ) {
      obj[propName] = filterFalseyValues(obj[propName]);
    }
  }
  return obj;
}

export function toFormData(data: Record<string, any>) {
  const formData = new FormData();
  buildFormData(formData, data);
  return formData;
}

const buildFormData = (form: FormData, data: Record<string, any>) => {
  Object.keys(data).forEach((x) => {
    if (data[x] instanceof Blob) {
      form.append(x, data[x], data[x].name);
    } else if (data[x] instanceof Date) {
      form.append(x, data[x].toString());
    } else {
      form.append(x, data[x]);
    }
  });
};

// function buildFormData(
//   formData: FormData,
//   data: Record<string, any>,
//   parentKey?: string
// ) {
//   if (
//     data &&
//     typeof data === "object" &&
//     !(data instanceof Date) &&
//     !(data instanceof Blob)
//   ) {
//     console.log(data);
//     Object.keys(data).forEach((key) => {
//       buildFormData(
//         formData,
//         data[key],
//         parentKey ? `${parentKey}[${key}]` : key
//       );
//     });
//   } else if (parentKey) {
//     const value =
//       data instanceof Date ? data.toString() : (data as string | Blob);
//     formData.append(parentKey, value);
//   }
// }

// function buildFormData
