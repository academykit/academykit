import TrainingTable from "@components/Course/TrainingTable";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import useAuth from "@hooks/useAuth";
import {
  Box,
  Center,
  Container,
  Flex,
  Group,
  Loader,
  rem,
  SegmentedControl,
} from "@mantine/core";
import { IconColumns, IconLayoutGrid } from "@tabler/icons-react";
import { CourseStatus, UserRole } from "@utils/enums";
import { useCourse } from "@utils/services/courseService";
import { useEffect, useState } from "react";
import { useTranslation } from "react-i18next";
import CourseList from "./component/List";

const CompletedCourseList = ({
  setInitialSearch,
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  useEffect(() => {
    setInitialSearch([
      {
        key: "Status",
        value: CourseStatus.Completed.toString(),
      },
    ]);
  }, []);
  const auth = useAuth();
  const { data, isLoading } = useCourse(searchParams);
  const role = auth?.auth?.role ?? UserRole.Trainee;
  const { t } = useTranslation();
  const [showCompletedList, setShowCompletedList] = useState(false);
  const [selectedView, setSelectedView] = useState("list");

  useEffect(() => {
    const timer = setTimeout(() => {
      setShowCompletedList(true);
    }, 1000); // 1 second

    return () => clearTimeout(timer);
  }, []);
  return (
    <Container fluid>
      <Container fluid>
        <Flex
          pb={20}
          style={{
            justifyContent: "end",
            alignItems: "center",
          }}
        >
          {searchComponent(t("search_trainings") as string)}
        </Flex>
      </Container>

      <Group justify="flex-end" my={30}>
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

      {data?.items &&
        (data.totalCount >= 1 ? (
          showCompletedList ? (
            selectedView === "table" ? (
              <TrainingTable courses={data?.items} search={searchParams} />
            ) : (
              <CourseList
                role={role}
                courses={data.items}
                search={searchParams}
              />
            )
          ) : (
            <Loader />
          )
        ) : (
          <Box>{t("no_trainings_found")}</Box>
        ))}
      {isLoading && <Loader />}
      {selectedView === "table" &&
        showCompletedList &&
        data &&
        pagination(data.totalPage, data.items.length)}
    </Container>
  );
};

export default withSearchPagination(CompletedCourseList);
