import { useForm } from "@mantine/form";
import { showNotification } from "@mantine/notifications";
import errorType from "@utils/services/axiosError";
import InlineInput from "./InlineInput";

const EditNameForm = ({
  item,
  slug,
  setIsEditing,
  updateFunction,
}: {
  item: any;
  slug: string;
  setIsEditing: Function;
  updateFunction: any;
}) => {
  const form = useForm({
    initialValues: {
      name: item.name,
    },
  });

  return (
    <form
      onSubmit={form.onSubmit(async (values) => {
        try {
          await updateFunction.mutateAsync({
            id: slug,
            sectionId: item.slug,
            sectionName: values.name,
          });
          showNotification({
            message: "Section Updated Successfully!",
            title: "Success",
          });
          setIsEditing(false);
        } catch (error) {
          const err = errorType(error);

          showNotification({
            message: err,
            title: "Error",
            color: "red",
          });
        }
      })}
    >
      <InlineInput
        placeholder="Enter section name"
        onCloseEdit={() => setIsEditing(false)}
        {...form.getInputProps("name")}
      ></InlineInput>
    </form>
  );
};

export default EditNameForm;
