import {
  ColorScheme,
  ColorSchemeProvider,
  MantineProvider,
} from "@mantine/core";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { useState } from "react";
import { NotificationsProvider } from "@mantine/notifications";
import AppRoutes from "@routes/AppRoutes";
import { AuthProvider } from "@context/AuthProvider";
import { COLOR_SCHEME_KEY } from "@utils/constants";
import { ReactQueryDevtools } from "@tanstack/react-query-devtools";
import { LayoutProvider } from "@context/LayoutProvider";
import ErrorBoundary from "@components/ErrorBoundry";
import { BrowserRouter } from "react-router-dom";
import ScrollToTop from "@components/ScrollToTop";

const App = () => {
  const [colorScheme, setColorScheme] = useState<ColorScheme>(
    (localStorage.getItem(COLOR_SCHEME_KEY) as "light" | "dark") ?? "light"
  );
  const toggleColorScheme = (value?: ColorScheme) => {
    const nextColorScheme =
      value || (colorScheme === "dark" ? "light" : "dark");
    setColorScheme(nextColorScheme);
    localStorage.setItem(COLOR_SCHEME_KEY, nextColorScheme);
  };
  const queryClient = new QueryClient();

  return (
    <ColorSchemeProvider
      colorScheme={colorScheme}
      toggleColorScheme={toggleColorScheme}
    >
      <BrowserRouter>
        <ScrollToTop />
        <MantineProvider
          withGlobalStyles
          withNormalizeCSS
          theme={{
            components: {
              Anchor: {
                styles: (theme) => ({
                  root: {
                    color: theme.colors.brand[7],
                  },
                }),
              },
            },
            colorScheme,
            fontFamily: "Poppins, sans-serif",
            fontFamilyMonospace: "Monaco, Courier, monospace",
            headings: { fontFamily: "Poppins, sans-serif" },
            loader: "dots",

            dateFormat: "MMM DD, YYYY",
            colors: {
              brand: [
                "#7AD1DD",
                "#5FCCDB",
                "#44CADC",
                "#2AC9DE",
                "#1AC2D9",
                "#11B7CD",
                "#09ADC3",
                "#0E99AC",
                "#128797",
                "#147885",
              ],
            },
            primaryColor: "brand",
          }}
        >
          <NotificationsProvider>
            <QueryClientProvider client={queryClient}>
              <ErrorBoundary>
                <AuthProvider>
                  <LayoutProvider>
                    <AppRoutes />
                  </LayoutProvider>
                </AuthProvider>
                <ReactQueryDevtools initialIsOpen={false} />
              </ErrorBoundary>
            </QueryClientProvider>
          </NotificationsProvider>
        </MantineProvider>
      </BrowserRouter>
    </ColorSchemeProvider>
  );
};

export default App;
