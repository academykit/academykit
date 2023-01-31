import ProgressBar from "@components/Ui/ProgressBar";
import withSearchPagination, {
  IWithSearchPagination,
} from "@hoc/useSearchPagination";
import {
  Title,
  Paper,
  Anchor,
  Table,
  useMantineTheme,
  Box,
  Loader,
} from "@mantine/core";
import RoutePath from "@utils/routeConstants";
import { useMyCourse } from "@utils/services/courseService";
import moment from "moment";
import { Link, useParams } from "react-router-dom";

const AttendedTrainings = ({
  searchParams,
  searchComponent,
  pagination,
}: IWithSearchPagination) => {
  const { id } = useParams();
  const { data, isLoading } = useMyCourse(id as string, searchParams);
  const theme = useMantineTheme();

  return (
    <div>
      {" "}
      <Title mt={10} size={30} mb={10}>
        Attended Trainings{" "}
      </Title>
      <Paper>
        <Table>
          <thead>
            <tr>
              <th>Title</th>
              <th>Enrolled Date</th>
              <th>Progress</th>
            </tr>
          </thead>
          <tbody>
            {data &&
              data.totalCount > 0 &&
              data.items.map((x) => (
                <tr>
                  <td>
                    <Anchor
                      component={Link}
                      to={RoutePath.courses.description(x.slug).route}
                    >
                      {x.name}
                    </Anchor>
                  </td>
                  <td>{moment(x.createdOn).format(theme.dateFormat)}</td>

                  <td>
                    <ProgressBar total={100} positive={x.percentage} />
                  </td>
                </tr>
              ))}
            {isLoading && <Loader />}
          </tbody>
        </Table>
      </Paper>
      {data && data.totalPage > 1 && pagination(data.totalPage)}
      {data && data.totalCount === 0 && <Box mt={5}>No Trainings Found</Box>}
    </div>
  );
};

export default withSearchPagination(AttendedTrainings);