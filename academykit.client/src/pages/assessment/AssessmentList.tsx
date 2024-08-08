import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Center,
  Group,
  rem,
  ScrollArea,
  SegmentedControl,
  SimpleGrid,
} from "@mantine/core";
import { IconColumns, IconLayoutGrid } from "@tabler/icons-react";
import { UserRole } from "@utils/enums";
import { useAssessments } from "@utils/services/assessmentService";
import { useState } from "react";
import { useTranslation } from "react-i18next";
import AssessmentTable from "./AssessmentTable";
import AssessmentCard from "./component/AssessmentCard";

const AssessmentList = ({
  searchComponent,
  searchParams,
  pagination,
  filterComponent,
}: IWithSearchPagination) => {
  const { t } = useTranslation();
  const assessmentData = useAssessments(searchParams);
  const auth = useAuth();
  const [selectedView, setSelectedView] = useState("list");

  return (
    <>
      <Group my={10}>
        <Box flex={1}>{searchComponent(t("search_assessments") as string)}</Box>

        {auth?.auth?.role !== UserRole.Trainee &&
          filterComponent(
            [
              { value: "1", label: t("Draft") },
              { value: "2", label: t("review") },
              { value: "3", label: t("Published") },
              { value: "5", label: t("rejected") },
            ],
            t("assessment_status"),
            "assessmentStatus"
          )}
        <SegmentedControl
          value={selectedView}
          onChange={setSelectedView}
          data={[
            {
              value: "list",
              label: (
                <Center style={{ gap: 10 }}>
                  <IconLayoutGrid style={{ width: rem(20), height: rem(20) }} />
                </Center>
              ),
            },
            {
              value: "table",
              label: (
                <Center style={{ gap: 10 }}>
                  <IconColumns style={{ width: rem(20), height: rem(20) }} />
                </Center>
              ),
            },
          ]}
        />
      </Group>

      <ScrollArea>
        {assessmentData &&
          assessmentData?.data?.items &&
          (assessmentData.data?.totalCount >= 1 ? (
            selectedView === "table" ? (
              <AssessmentTable
                assessment={assessmentData.data.items}
                userRole={auth?.auth?.role as UserRole}
                currentUser={(auth?.auth?.id as string) ?? ""}
              />
            ) : (
              <SimpleGrid cols={{ base: 1, sm: 2, lg: 3 }}>
                {assessmentData.data?.items.map((assessmentData) => (
                  <AssessmentCard
                    key={assessmentData.id}
                    data={assessmentData}
                    userRole={auth?.auth?.role as UserRole}
                    currentUser={(auth?.auth?.id as string) ?? ""}
                  />
                ))}
              </SimpleGrid>
            )
          ) : (
            <Box>{t("no_assessment_found")}</Box>
          ))}
      </ScrollArea>

      {selectedView === "table" &&
        assessmentData.data &&
        pagination(
          assessmentData.data?.totalPage,
          assessmentData.data?.items.length
        )}
    </>
  );
};

export default withSearchPagination(AssessmentList);
