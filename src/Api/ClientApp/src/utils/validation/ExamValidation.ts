import * as Yup from "yup";

const examVaslidation = Yup.object().shape({
  name: Yup.string().required("Title of Question is required"),
  tags: Yup.array().min(1, "One Tag is required"),

  answers: Yup.array()
    .of(
      Yup.object().shape({
        options: Yup.string().trim().required("Options is required"),
      })
    )
    .when(["type"], {
      is: "0",
      then: Yup.array()
        .min(2, "Options shoukd be more than one")
        .test(
          "test",
          "On checkbox type at least one option should be selected ",
          function (value: any) {
            const a = value?.filter((x: any) => x.isCorrect).length !== 1;
            return !a;
          }
        ),
      otherwise: Yup.array().test(
        "test",
        "On radio only one option should be selected ",
        function (value) {
          return value?.filter((x) => x.isCorrect).length === 1;
        }
      ),
    }),
});

export default examVaslidation;
