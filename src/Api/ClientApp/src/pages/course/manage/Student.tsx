import {
  Table,
  Anchor,
  ScrollArea,
  Badge,
  UnstyledButton,
  Box,
  Paper,
  Avatar,
} from "@mantine/core";
import { Link, useParams } from "react-router-dom";
import ProgressBar from "@components/Ui/ProgressBar";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  IStudentStat,
  useGetStudentStatistics,
} from "@utils/services/manageCourseService";
import RoutePath from "@utils/routeConstants";

const ManageStudents = ({
  searchParams,
  pagination,
  searchComponent,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const course_id = id as string;

  const getStudentStat = useGetStudentStatistics(course_id, searchParams);

  const Rows = ({ item }: { item: IStudentStat }) => {
    return (
      <tr key={item.userId}>
        <td>
          <Anchor
            component={Link}
            to={`${RoutePath.userProfile}/${item.userId}`}
            size="sm"
            sx={{ display: "flex" }}
          >
            <Avatar
              size={26}
              mr={8}
              src={
                "https://d2j66931zkyzgl.cloudfront.net/standalone/2eb8a5be-218a-43a6-b131-17531d669064.png"
              }
              radius={26}
            />

            {item?.fullName}
          </Anchor>
        </td>
        <td>
          <ProgressBar total={100} positive={item?.percentage} />
        </td>
        <td style={{ textAlign: "center" }}>
          <Anchor
            component={Link}
            to={`${RoutePath.classes}/${course_id}/${item.lessonSlug}`}
            size="sm"
          >
            {item?.lessonName}
          </Anchor>
        </td>
        <td>
          <UnstyledButton component={Link} to={item.userId}>
            <Badge color="green" variant="outline">
              View
            </Badge>
          </UnstyledButton>
        </td>
      </tr>
    );
  };

  return (
    <ScrollArea>
      <div style={{ display: "flex" }}>
        <Box mx={3} sx={{ width: "100%" }}>
          {searchComponent("Search for students")}
        </Box>
      </div>
      {getStudentStat.data && getStudentStat.data?.totalCount > 0 ? (
        <Paper mt={10}>
          <Table
            sx={{ minWidth: 800 }}
            verticalSpacing="xs"
            striped
            highlightOnHover
          >
            <thead>
              <tr>
                <th>Name</th>
                <th>Progress</th>
                <th style={{ textAlign: "center" }}>Current Lesson</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {getStudentStat.data?.items?.map((item: any) => (
                <Rows item={item} key={item?.userId} />
              ))}
            </tbody>
          </Table>
        </Paper>
      ) : (
        <Box mt={5}>No Students Found</Box>
      )}
      {getStudentStat.data && pagination(getStudentStat.data?.totalPage)}
    </ScrollArea>
  );
};

export default withSearchPagination(ManageStudents);
