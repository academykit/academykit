import useAuth from "@hooks/useAuth";
import { Box, Button, Group, Loader, Title } from "@mantine/core";
import { UserRole } from "@utils/enums";
import RoutePath from "@utils/routeConstants";
import { Suspense } from "react";
import { useTranslation } from "react-i18next";
import { Link, Outlet } from "react-router-dom";

const AssessmentLayout = () => {
  const auth = useAuth();
  const { t } = useTranslation();
  const role = auth?.auth?.role ?? UserRole.Trainee;
  return (
    <>
      <Box>
        <Group
          style={{ justifyContent: "space-between", alignItems: "center" }}
        >
          <Title style={{ flexGrow: 2 }}>{t("assessments")}</Title>
          {role <= UserRole.Trainer && (
            <Link to={RoutePath.assessment.create}>
              <Button>{t("new_assessment")}</Button>
            </Link>
          )}
        </Group>
      </Box>
      <Suspense fallback={<Loader />}>
        <Outlet />
      </Suspense>
    </>
  );
};

export default AssessmentLayout;
